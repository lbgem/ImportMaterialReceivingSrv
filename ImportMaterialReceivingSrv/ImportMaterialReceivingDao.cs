using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ImportMaterialReceivingSrv
{
    public class ImportMaterialReceivingDao
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ImportMaterialReceivingController));
        static String connOrcleString = ConfigurationManager.ConnectionStrings["sqlDB"].ToString();

        public string LoadCollections(DataTable collections)
        {
            string error = "";
            error=insertOracle(collections, "select * from SD_IMPORTMATERIALRECEIVING_ORDER");
            if(error.Length != 0)
            {
                error = "Collections数据加载失败:" + error;
                logger.Info("Collections数据加载失败:" +error);
            }
            logger.Info("Collections数据加载成功");
            return error;
        }

        public string LoadInfos(DataTable infos)
        {
            string error = "";
            error=insertOracle(infos, "select * from SD_IMPORTMATERIALRECEIVING_DETAIL");
            if (error.Length != 0)
            {
                error = "Infos数据加载失败:" + error;
                logger.Info("Infos数据加载失败"+error);
            }
            logger.Info("Infos数据加载成功");
            return error;
        }

        public string CheckData(List<ErrorCollection> errorCollections)
        {
            long startTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            string error = "";
            OracleConnection conn=OpenConn();
            OracleCommand orm = conn.CreateCommand();
            orm.CommandType = CommandType.StoredProcedure;
            orm.CommandText = "proc1";
            try
            {
                orm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                error = "核验执行失败"+e.Message;
                logger.Info("核验执行失败:"+ e.Message);
                conn.Close();
            }

            var cmd = conn.CreateCommand();
            cmd.CommandText = "select * from SD_IMPORTMATERIALRECEIVING_ERROR";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    ErrorCollection errorCollection = new ErrorCollection();
                    errorCollection.ERROR_FLAG = changeNull(reader["ERROR_FLAG"].ToString());
                    errorCollection.ERROR_PRI_KEY = changeNull(reader["ERROR_PRI_KEY"]);
                    errorCollection.ERROR_MESSAGE = changeNull(reader["ERROR_MESSAGE"]);
                    errorCollection.RESERVED_1 = changeNull(reader["RESERVED_1"]);
                    errorCollection.RESERVED_2 = changeNull(reader["RESERVED_2"]);
                    errorCollections.Add(errorCollection);
                }
            }
            catch (Exception e)
            {
                error="库表读取失败:" + e.Message;
                logger.Info("库表读取失败:"+e.Message);
                conn.Close();
                return error;
            }
            if (errorCollections.Count!=0)
            {
                error = "数据核验失败:";
                logger.Info("数据核验失败");
                conn.Close();
                return error;
            }
            logger.Info("数据核验成功");
            conn.Close();
            return error;
        }

        public String changeNull(Object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        static OracleConnection OpenConn()
        {
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = connOrcleString;
            conn.Open();
            logger.Info("database open");
            return conn;
        }

        static void CloseConn(OracleConnection conn)
        {
            if (conn == null) { return; }
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                logger.Info(e.Message);
            }
            finally
            {
                conn.Dispose();
            }
        }
        public string insertOracle(DataTable dataTable, string sql) //  Oracle sql 查询的是表头
        {
            string error = "";
            string ConnStr = connOrcleString;
            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                OracleTransaction m_OraTrans = conn.BeginTransaction();//创建事务对象
                try
                {
                    OracleCommand cmd = new OracleCommand(sql, conn);
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    OracleCommandBuilder cb = new OracleCommandBuilder(adapter);
                    DataTable dsNew = new DataTable();
                    int count = adapter.Fill(dsNew);
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        DataRow dr = dsNew.NewRow();
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            dr[dsNew.Columns[j].ColumnName] = dataTable.Rows[i][j];
                        }
                        dsNew.Rows.Add(dr);
                    }
                    count = adapter.Update(dsNew);
                    adapter.UpdateBatchSize = 5000;
                    m_OraTrans.Commit();
                    //adapter.Update(dataTable);
                    return error;
                }
                catch (Exception e)
                {
                    m_OraTrans.Rollback();
                    //LogHelper.WriteErrLog("insertOracle", ex.Message);
                    error = e.Message;
                    logger.Info(e.Message);
                    return error;
                }
            }
        }

    }
}