using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace ImportMaterialReceivingSrv
{
    /// <summary>
    ///基於.net（ 向下兼容4.0）開發 OracleDBHelper工具類
    ///<para>作者： 害羞的青蛙</para>
    ///<para>時間： 2019-12-6</para>
    /// </summary>
    public class OracleDBHelper
    {
        /// <summary>
        /// 執行SQL語句返回DataTable
        /// </summary>
        /// <param name="SQL">SQL語句</param>
        /// <param name="DBUrl">數據庫鏈接地址</param>
        /// <returns></returns>
        public DataTable GetDataTableBySQL(string SQL, string DBUrl)
        {
            if (DBUrl.ToString().Trim() == "" || DBUrl == null) throw new Exception("數據庫鏈接地址不能為空");
            // 获取与数据库的连接对象並且绑定连接字符串
            Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection(DBUrl);
            conn.Open();//打開資源
                        //获取数据库操作对象
            Oracle.ManagedDataAccess.Client.OracleCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = SQL;
                Oracle.ManagedDataAccess.Client.OracleDataAdapter adapter = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataTable.TableName = "數據集";
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                return dataTable;
            }
            catch (Exception ex)
            {
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                throw ex;
            }
        }
        /// <summary>
        /// 執行非查詢的SQL語句
        /// </summary>
        /// <param name="SQL">SQL語句</param>
        /// <param name="DBUrl">數據庫鏈接地址</param>
        /// <returns></returns>
        public int GetNonQueryBySQL(string SQL, string DBUrl)
        {
            if (DBUrl.ToString().Trim() == "" || DBUrl == null) throw new Exception("數據庫鏈接地址不能為空");
            // 获取与数据库的连接对象並且绑定连接字符串
            Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection(DBUrl);
            conn.Open();//打開資源
                        //获取数据库操作对象
            Oracle.ManagedDataAccess.Client.OracleCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = SQL;
                int num = cmd.ExecuteNonQuery();
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                return num;
            }
            catch (Exception ex)
            {
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                throw ex;
            }
        }
        /// <summary>
        /// 執行多條SQL語句，實現數據庫事務。
        /// </summary>
        /// <param name="SQLStringList">多條SQL語句</param>       
        /// <param name="DBUrl">數據庫鏈接地址</param>    
        public int GetNonQueryByManySQL(ArrayList SQLStringList, string DBUrl)
        {
            if (DBUrl.ToString().Trim() == "" || DBUrl == null) throw new Exception("數據庫鏈接地址不能為空");
            using (Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection(DBUrl))
            {
                conn.Open();
                Oracle.ManagedDataAccess.Client.OracleCommand cmd = new Oracle.ManagedDataAccess.Client.OracleCommand();
                cmd.Connection = conn;
                Oracle.ManagedDataAccess.Client.OracleTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int num = 0;
                    for (int i = 0; i < SQLStringList.Count; i++)
                    {
                        string SQL = SQLStringList[i].ToString();//獲取SQL語句
                        if (SQL.Trim().Length > 1)
                        {
                            cmd.CommandText = SQL;
                            num = cmd.ExecuteNonQuery();
                        }
                        tx.Commit();//提交事務
                        cmd.Dispose();//釋放資源
                        conn.Dispose();//釋放資源
                        conn.Close();//關閉

                    }
                    return num;//返回執行結果數量
                }
                catch (Oracle.ManagedDataAccess.Client.OracleException E)
                {
                    tx.Rollback();//事務回滾
                    throw new Exception(E.Message);
                }
            }
        }
        /// <summary>
        /// 調用存儲返回單個游標結果集（最後一個位置必須為游標，位置不能顛倒）
        /// <para>obj使用方法：new{ v_data=value, v_data1=value1,out_cursor=""}</para>
        /// <para>注意：obj中v_data為存儲參數名稱，value為對應的值， out_cursor為游標不需要輸入值</para>
        /// </summary>
        /// <param name="storageName">存儲名稱</param>
        /// <param name="DBUrl">數據庫鏈接地址</param>
        /// <param name="obj">存儲參數對象</param>
        /// <returns></returns>
        public DataTable GetDataTableByStorageName(string storageName, string DBUrl, object obj)
        {
            if (DBUrl.ToString().Trim() == "" || DBUrl == null) throw new Exception("數據庫鏈接地址不能為空");
            // 获取与数据库的连接对象並且绑定连接字符串
            Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection(DBUrl);
            conn.Open();//打開資源
            //获取数据库操作对象
            Oracle.ManagedDataAccess.Client.OracleCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = storageName;//存儲名稱
                cmd.CommandType = CommandType.StoredProcedure;
                PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);//獲取object中的字段名和值
                for (int i = 0; i < properties.Length; i++)
                {
                    if (i == (properties.Length - 1))
                    {//設定輸出的類型和值
                        cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                    }
                    else
                    {//設定輸入的類型和值
                        cmd.Parameters.Add(properties[i].Name, GetOracleDbType(properties[i], obj)).Direction = ParameterDirection.Input;
                        cmd.Parameters[properties[i].Name].Value = properties[i].GetValue(obj, null);//賦值
                    }
                }
                DataTable dataTable = new DataTable();
                Oracle.ManagedDataAccess.Client.OracleDataAdapter oda = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(cmd);
                oda.Fill(dataTable);
                dataTable.TableName = "數據集";
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                return dataTable;
            }
            catch (Exception ex)
            {
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                throw ex;
            }
        }
        /// <summary>
        /// 調用存儲返回String字符串信息（最後一個位置必須為String類型字符，位置不能顛倒）
        /// <para>obj使用方法：new{ v_data=value, v_data1=value1,out_string=""}</para>
        /// <para>注意：obj中v_data為存儲參數名稱，value為對應的值，out_string為輸出參數不需要輸入值</para>
        /// </summary>
        /// <param name="storageName"></param>
        /// <param name="DBUrl"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetStringDataByStorageName(string storageName, string DBUrl, object obj)
        {
            if (DBUrl.ToString().Trim() == "" || DBUrl == null) throw new Exception("數據庫鏈接地址不能為空");
            Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection(DBUrl);
            conn.Open();
            //获取数据库操作对象
            Oracle.ManagedDataAccess.Client.OracleCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = storageName;//存儲名稱
                cmd.CommandType = CommandType.StoredProcedure;
                PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);//獲取object中的字段名和值
                for (int i = 0; i < properties.Length; i++)
                {
                    if (i == (properties.Length - 1))
                    { //設定輸出的類型和值
                        cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, short.MaxValue).Direction = ParameterDirection.Output;
                        cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                    }
                    else
                    {//設定輸入的類型和值
                        cmd.Parameters.Add(properties[i].Name, GetOracleDbType(properties[i], obj)).Direction = ParameterDirection.Input;
                        cmd.Parameters[properties[i].Name].Value = properties[i].GetValue(obj, null);//賦值
                    }
                }
                cmd.ExecuteNonQuery();
                string message = cmd.Parameters[properties[properties.Length - 1].Name].Value.ToString();//獲取返回的值
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                return message;
            }
            catch (Exception ex)
            {
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                throw ex;
            }
        }
        /// <summary>
        /// 調用存儲返回String字符串信息和DataTable數據表格（最後兩個位置必須為返回參數，一個為輸出字符串另一個為游標，位置不能顛倒）
        /// <para>obj使用方法：new{ v_data=value, v_data1=value1,out_string="",out_cursor=""}</para>
        /// <para>注意：obj中v_data為存儲參數名稱，value為對應的值，out_string為輸出參數不需要輸入值， out_cursor為游標不需要輸入值</para>
        /// </summary>
        /// <param name="storageName">存儲名稱</param>
        /// <param name="DBUrl">數據庫鏈接地址</param>
        /// <param name="obj">存儲參數對象</param>
        /// <param name="dataTable">返回結果集</param>
        /// <returns></returns>
        public string GetStringAndDataTableByStorageName(string storageName, string DBUrl, object obj, out DataTable dataTable)
        {
            if (DBUrl.ToString().Trim() == "" || DBUrl == null) throw new Exception("數據庫鏈接地址不能為空");
            // 获取与数据库的连接对象並且绑定连接字符串
            Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection(DBUrl);
            conn.Open();//打開資源
            //获取数据库操作对象
            Oracle.ManagedDataAccess.Client.OracleCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = storageName;//存儲名稱
                cmd.CommandType = CommandType.StoredProcedure;
                PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);//獲取object中的字段名和值
                for (int i = 0; i < properties.Length; i++)
                {
                    if (i == (properties.Length - 2))
                    {//設定輸出的類型和值
                        cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, short.MaxValue).Direction = ParameterDirection.Output;
                        cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                    }
                    else if (i == (properties.Length - 1))
                    {//設定輸出的類型和值
                        cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                    }
                    else
                    {//設定輸入的類型和值
                        cmd.Parameters.Add(properties[i].Name, GetOracleDbType(properties[i], obj)).Direction = ParameterDirection.Input;
                        cmd.Parameters[properties[i].Name].Value = properties[i].GetValue(obj, null);//賦值
                    }
                }
                DataTable dt = new DataTable();
                dt.TableName = "數據集";
                Oracle.ManagedDataAccess.Client.OracleDataAdapter oda = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(cmd);
                oda.Fill(dt);
                dataTable = dt;//返回數據結果集
                string message = cmd.Parameters[properties[properties.Length - 2].Name].Value.ToString();//獲取輸出的字符串
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                return message;
            }
            catch (Exception ex)
            {
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                throw ex;
            }
        }
        /// <summary>
        /// 調用存儲返回String字符串信息和DataTable數據表格（最後兩個位置必須為返回參數，一個為輸出字符串另一個為游標，位置不能顛倒）
        /// <para>obj使用方法：new{ v_data=value, v_data1=value1,out_string="",out_cursor=""}</para>
        /// <para>注意：obj中v_data為存儲參數名稱，value為對應的值，out_string為輸出參數不需要輸入值， out_cursor為游標不需要輸入值</para>
        /// <para>outType為true輸出參數在前，為flase輸出參數在後；cursorType為true游標在輸出字符串前，為flase游標輸出字符串在後</para>
        /// </summary>
        /// <param name="storageName">存儲名稱</param>
        /// <param name="DBUrl">數據庫鏈接地址</param>
        /// <param name="outType">輸出狀態</param>
        /// <param name="cursorType">游標狀態</param>
        /// <param name="obj">存儲參數對象</param>
        /// <param name="dataTable">返回結果集</param>
        /// <returns></returns>
        public string GetStringAndDataTableByStorageName(string storageName, bool outType, bool cursorType, string DBUrl, object obj, out DataTable dataTable)
        {
            if (DBUrl.ToString().Trim() == "" || DBUrl == null) throw new Exception("數據庫鏈接地址不能為空");
            // 获取与数据库的连接对象並且绑定连接字符串
            Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection(DBUrl);
            conn.Open();//打開資源
            //获取数据库操作对象
            Oracle.ManagedDataAccess.Client.OracleCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = storageName;//存儲名稱
                cmd.CommandType = CommandType.StoredProcedure;
                PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);//獲取object中的字段名和值
                for (int i = 0; i < properties.Length; i++)
                {
                    if (outType)
                    {
                        if (cursorType)
                        {
                            switch (i)
                            {
                                case 0:
                                    cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                    cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                                    break;
                                case 1:
                                    cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, short.MaxValue).Direction = ParameterDirection.Output;
                                    cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                                    break;
                                default:
                                    //設定輸入的類型和值
                                    cmd.Parameters.Add(properties[i].Name, GetOracleDbType(properties[i], obj)).Direction = ParameterDirection.Input;
                                    cmd.Parameters[properties[i].Name].Value = properties[i].GetValue(obj, null);//賦值
                                    break;
                            }
                        }
                        else
                        {
                            switch (i)
                            {
                                case 0:
                                    cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, short.MaxValue).Direction = ParameterDirection.Output;
                                    cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                                    break;
                                case 1:
                                    cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                    cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                                    break;
                                default:
                                    //設定輸入的類型和值
                                    cmd.Parameters.Add(properties[i].Name, GetOracleDbType(properties[i], obj)).Direction = ParameterDirection.Input;
                                    cmd.Parameters[properties[i].Name].Value = properties[i].GetValue(obj, null);//賦值
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (cursorType)
                        {
                            if (i == (properties.Length - 2))
                            {
                                //設定輸出的類型和值
                                cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                            }
                            else if (i == (properties.Length - 1))
                            {
                                //設定輸出的類型和值
                                cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, short.MaxValue).Direction = ParameterDirection.Output;
                                cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                            }
                            else
                            {
                                //設定輸入的類型和值
                                cmd.Parameters.Add(properties[i].Name, GetOracleDbType(properties[i], obj)).Direction = ParameterDirection.Input;
                                cmd.Parameters[properties[i].Name].Value = properties[i].GetValue(obj, null);//賦值
                            }
                        }
                        else
                        {
                            if (i == (properties.Length - 2))
                            {//設定輸出的類型和值
                                cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, short.MaxValue).Direction = ParameterDirection.Output;
                                cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                            }
                            else if (i == (properties.Length - 1))
                            {//設定輸出的類型和值
                                cmd.Parameters.Add(properties[i].Name, Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                cmd.Parameters[properties[i].Name].Value = DBNull.Value;//賦值
                            }
                            else
                            {//設定輸入的類型和值
                                cmd.Parameters.Add(properties[i].Name, GetOracleDbType(properties[i], obj)).Direction = ParameterDirection.Input;
                                cmd.Parameters[properties[i].Name].Value = properties[i].GetValue(obj, null);//賦值
                            }
                        }
                    }
                }
                DataTable dt = new DataTable();
                dt.TableName = "數據集";
                Oracle.ManagedDataAccess.Client.OracleDataAdapter oda = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(cmd);
                oda.Fill(dt);
                dataTable = dt;//返回數據結果集
                string message = null;
                if (outType)
                {
                    if (cursorType)
                    {
                        message = cmd.Parameters[properties[1].Name].Value.ToString();//獲取輸出的字符串
                    }
                    else
                    {
                        message = cmd.Parameters[properties[2].Name].Value.ToString();//獲取輸出的字符串
                    }
                }
                else
                {
                    if (cursorType)
                    {
                        message = cmd.Parameters[properties[properties.Length - 2].Name].Value.ToString();//獲取輸出的字符串
                    }
                    else
                    {
                        message = cmd.Parameters[properties[properties.Length - 1].Name].Value.ToString();//獲取輸出的字符串
                    }
                }
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                return message;
            }
            catch (Exception ex)
            {
                cmd.Dispose();//釋放資源
                conn.Dispose();//釋放資源
                conn.Close();//關閉
                throw ex;
            }
        }
        ///// <summary>
        ///// 以現有表格數據批量添加數據
        ///// </summary>
        ///// <param name="dt">數據表格</param>
        ///// <param name="FormName">數據表名</param>
        ///// <param name="DBUrl">數據庫鏈接地址</param>
        ///// <returns></returns>
        //public bool AddInBatchesToDataTable(DataTable dt, string FormName, string DBUrl)
        //{
        //    try
        //    {
        //        if (FormName.ToString().Trim() == "") throw new Exception("數據表名不能为空");
        //        System.Diagnostics.Debug.WriteLine("----------------------開始執行--------------------------");
        //        System.Diagnostics.Debug.WriteLine("執行表明：" + dt.TableName + "--------------------------");
        //        long star = Convert.ToInt64(System.DateTime.Now.ToString("yyyyMMddHHmmssfff"));
        //        Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection(DBUrl);
        //        OracleBulkCopy bulkCopy = new OracleBulkCopy(DBUrl, OracleBulkCopyOptions.UseInternalTransaction);
        //        bulkCopy.BatchSize = 100000;
        //        bulkCopy.BulkCopyTimeout = 260;
        //        bulkCopy.DestinationTableName = FormName;    //服务器上目标表的名称
        //        bulkCopy.BatchSize = dt.Rows.Count;   //每一批次中的行数
        //        try
        //        {
        //            conn.Open();
        //            if (dt != null && dt.Rows.Count != 0)
        //                bulkCopy.WriteToServer(dt);   //将提供的数据源中的所有行复制到目标表中
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            conn.Close();
        //            if (bulkCopy != null)
        //                bulkCopy.Close();
        //        }
        //        long end = Convert.ToInt64(System.DateTime.Now.ToString("yyyyMMddHHmmssfff"));
        //        System.Diagnostics.Debug.WriteLine("共添加：" + dt.Rows.Count + "條數數據----耗時：" + ((end - star) / 10000) + "." + ((end - star) % 10000) + "秒");
        //        System.Diagnostics.Debug.WriteLine("----------------------執行結束--------------------------");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        /// <summary>
        /// 將輸入參數的類型與Oracle中的參數類型轉化一致
        /// </summary>
        /// <param name="propertyInfo">屬性特征</param>
        /// <param name="obj">對象</param>
        /// <returns></returns>
        private Oracle.ManagedDataAccess.Client.OracleDbType GetOracleDbType(PropertyInfo propertyInfo, object obj)
        {
            try
            {
                Oracle.ManagedDataAccess.Client.OracleDbType oracleDbType = new Oracle.ManagedDataAccess.Client.OracleDbType();
                switch (propertyInfo.GetValue(obj, null).GetType().Name)
                {
                    case "String":
                        oracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.NVarchar2;
                        break;
                    case "Int32":
                        oracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Int32;
                        break;
                    case "Int64":
                        oracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Int64;
                        break;
                    case "DateTime":
                        oracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Date;
                        break;
                    case "Blob":
                        oracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Blob;
                        break;
                    case "Double":
                        oracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Double;
                        break;
                    case "Decimal":
                        oracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Decimal;
                        break;
                    case "Long":
                        oracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Long;
                        break;
                    case "Boolean":
                        oracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Boolean;
                        break;
                    case "BinaryFloat":
                        oracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.BinaryFloat;
                        break;
                }
                return oracleDbType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}