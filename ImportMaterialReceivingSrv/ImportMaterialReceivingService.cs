using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace ImportMaterialReceivingSrv
{
    public class ImportMaterialReceivingService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ImportMaterialReceivingSrv));
        ImportMaterialReceivingDao importMaterialReceivingDao = new ImportMaterialReceivingDao();

        public MsgHeader GetMsgheader(ImportMaterialReceivingSrvRequest importMaterialReceivingSrvRequest)
        {
            MsgHeader msgHeader = new MsgHeader();
            try
            {
                msgHeader = importMaterialReceivingSrvRequest.MsgHeader;
            }
            catch(Exception e)
            {
                logger.Info("获取MsgHeader失败:"+e.Message);
            }
            return msgHeader;
        }
        public String CheckMsgheader(MsgHeader msgHeader)
        {
            String SOURCESYSTEMID;
            String SOURCESYSTEMNAME;
            String TOKEN;
            string SOURCESYSTEMID_CHECK = ConfigurationManager.AppSettings["SOURCESYSTEMID"];
            string SOURCESYSTEMNAME_CHECK = ConfigurationManager.AppSettings["SOURCESYSTEMNAME"];
            String token = "PERIPHERAL_SYSTEM_SCM_TOKEN" + DateTime.Now.ToString("yyyyMMdd");
            String md5token = "";
            String error = "";
            try
            {
                md5token = EncoderByMd5(token);
                logger.Info("获取到今天的Token:" + md5token);
            }
            catch (Exception e)
            {
                logger.Info("获取今天的Token失败:"+e.Message);
                error = "Token获取错误;";
                return error;
            }
            try
            {
                SOURCESYSTEMID=msgHeader.SOURCESYSTEMID;
                SOURCESYSTEMNAME= msgHeader.SOURCESYSTEMNAME;
                TOKEN= msgHeader.TOKEN;
            }
            catch (Exception e)
            {
                logger.Info("解析MsgHeader失败:" + e.Message);
                error = "MsgHeader解析错误;";
                return error;
            }


            if (String.IsNullOrEmpty(msgHeader.SOURCESYSTEMID)||!msgHeader.SOURCESYSTEMID.Equals(SOURCESYSTEMID_CHECK))
            {
                error += "服务消费方ID错误;";
            }
            if (String.IsNullOrEmpty(msgHeader.SOURCESYSTEMNAME) || !msgHeader.SOURCESYSTEMNAME.Equals(SOURCESYSTEMNAME_CHECK))
            {
                error += "服务消费方名称错误;";
            }
            if (String.IsNullOrEmpty(msgHeader.TOKEN) || !msgHeader.TOKEN.Equals(md5token))
            {
                error += "令牌认证信息错误;";
            }
            return error;
        }

        internal void insertMsgHeader(MsgHeader msgHeader)
        {
            throw new NotImplementedException();
        }

        public List<ImportMaterialReceivingSrvCollection> GetCollection(ImportMaterialReceivingSrvRequest importMaterialReceivingSrvRequest)
        {
            List<ImportMaterialReceivingSrvCollection> importMaterialReceivingSrvCollection = new List<ImportMaterialReceivingSrvCollection>();
            try
            {
                importMaterialReceivingSrvCollection = importMaterialReceivingSrvRequest.SrvCollection;
            }
            catch (Exception e)
            {
                logger.Info("获取Collection失败:" + e.Message);
            }
            return importMaterialReceivingSrvCollection;
        }

        public string CheckCollection(List<ImportMaterialReceivingSrvCollection> importMaterialReceivingSrvCollection)
        {
            String error = "";
            String PRI_KEY;
            String BILL_TYPE;
            String IS_DELIVERY;
            String BILL_CODE;
            String CITY_CODE;
            String RECEIVER_NUM;
            DateTime RECEIVER_DATE;
            List<LineInfo> lineInfo =new List<LineInfo>();
            try
            {
                foreach (ImportMaterialReceivingSrvCollection importMaterialReceivingSrvItem in importMaterialReceivingSrvCollection)
                {
                    PRI_KEY = importMaterialReceivingSrvItem.PRI_KEY;
                    BILL_TYPE = importMaterialReceivingSrvItem.BILL_TYPE;
                    IS_DELIVERY = importMaterialReceivingSrvItem.IS_DELIVERY;
                    BILL_CODE = importMaterialReceivingSrvItem.BILL_CODE;
                    CITY_CODE = importMaterialReceivingSrvItem.CITY_CODE;
                    RECEIVER_NUM = importMaterialReceivingSrvItem.RECEIVER_NUM;
                    RECEIVER_DATE = importMaterialReceivingSrvItem.RECEIVER_DATE;
                    lineInfo = GetLineInfo(importMaterialReceivingSrvItem);
                    error += CheckLineInfo(lineInfo);
                }

            }
            catch(Exception e)
            {
                logger.Info("解析Collection失败:" + e.Message);
                error += "解析Collection失败;";
                return error;
            }
            if (importMaterialReceivingSrvCollection.Count == 0)
            {
                logger.Info("解析Collection失败:条目为0");
                error += "Collection为空;";
                return error;
            }
            return error;
        }

        public List<LineInfo> GetLineInfo(ImportMaterialReceivingSrvCollection importMaterialReceivingSrvItem)
        {
            List<LineInfo> lineInfo = new List<LineInfo>();
            try
            {
                lineInfo = importMaterialReceivingSrvItem.LineInfo;
            }
            catch (Exception e)
            {
                logger.Info("获取LineInfo失败:" + e.Message);
            }
            return lineInfo;
        }

        private string CheckLineInfo(List<LineInfo> lineInfo)
        {
            string error = "";
            String PRI_KEY;
            String HEADER_PRI_KEY;
            String ITEM_NUMBER;
            String ITEM_DESCRIPTION;
            String UNIT_OF_MEASURE;
            String SUBINVENTORY_CODE;
            String SUBINVENTORY_DESC;
            String LOT_NUMBER;
            int QUANTITY;
            String BUDGET_PRI_KEY;
            String HTBM_VENDOR_PRODUCT_ID;
            String TERMINAL_CATEGORY;
            String HTBM_LOT_NUM;
            try
            {
                foreach (LineInfo lineInfoItem in lineInfo)
                {
                    PRI_KEY=lineInfoItem.PRI_KEY;
                    HEADER_PRI_KEY = lineInfoItem.HEADER_PRI_KEY;
                    ITEM_NUMBER = lineInfoItem.ITEM_NUMBER;
                    ITEM_DESCRIPTION = lineInfoItem.ITEM_DESCRIPTION;
                    UNIT_OF_MEASURE = lineInfoItem.UNIT_OF_MEASURE;
                    SUBINVENTORY_CODE = lineInfoItem.SUBINVENTORY_CODE;
                    SUBINVENTORY_DESC = lineInfoItem.SUBINVENTORY_DESC;
                    LOT_NUMBER = lineInfoItem.LOT_NUMBER;
                    QUANTITY = lineInfoItem.QUANTITY;
                    BUDGET_PRI_KEY = lineInfoItem.BUDGET_PRI_KEY;
                    HTBM_VENDOR_PRODUCT_ID = lineInfoItem.HTBM_VENDOR_PRODUCT_ID;
                    TERMINAL_CATEGORY = lineInfoItem.TERMINAL_CATEGORY;
                    HTBM_LOT_NUM = lineInfoItem.HTBM_LOT_NUM;
                }
            }
            catch (Exception e)
            {
                logger.Info("解析LineInfo失败:" + e.Message);
                error += "解析LineInfo失败;";
                return error;
            }
            if (lineInfo.Count == 0)
            {
                logger.Info("LineInfo:条目为0");
                error += "LineInfo为空;";
                return error;
            }
            return error;
        }

        public String TransformData(MsgHeader msgHeader,List<ImportMaterialReceivingSrvCollection> importMaterialReceivingSrvCollection, DataTable Collections,DataTable Infos)
        {
            string error = "";
            Collections.Columns.Add("PRI_KEY",typeof(String));
            Collections.Columns.Add("BILL_TYPE", typeof(String));
            Collections.Columns.Add("IS_DELIVERY", typeof(String));
            Collections.Columns.Add("BILL_CODE", typeof(String));
            Collections.Columns.Add("CITY_CODE", typeof(String));
            Collections.Columns.Add("RECEIVER_NUM", typeof(String));
            Collections.Columns.Add("RECEIVER_DATE", typeof(DateTime));

            Collections.Columns["PRI_KEY"].MaxLength = 60;
            Collections.Columns["BILL_TYPE"].MaxLength = 2;
            Collections.Columns["IS_DELIVERY"].MaxLength = 2;
            Collections.Columns["BILL_CODE"].MaxLength = 60;
            Collections.Columns["CITY_CODE"].MaxLength = 60;
            Collections.Columns["RECEIVER_NUM"].MaxLength = 60;


            Infos.Columns.Add("PRI_KEY",typeof(String));
            Infos.Columns.Add("HEADER_PRI_KEY",typeof(String));
            Infos.Columns.Add("ITEM_NUMBER",typeof(String));
            Infos.Columns.Add("ITEM_DESCRIPTION",typeof(String));
            Infos.Columns.Add("UNIT_OF_MEASURE", typeof(String));
            Infos.Columns.Add("SUBINVENTORY_CODE", typeof(String));
            Infos.Columns.Add("SUBINVENTORY_DESC", typeof(String));
            Infos.Columns.Add("LOT_NUMBER", typeof(String));
            Infos.Columns.Add("QUANTITY",typeof(int));
            Infos.Columns.Add("BUDGET_PRI_KEY", typeof(String));
            Infos.Columns.Add("HTBM_VENDOR_PRODUCT_ID", typeof(String));
            Infos.Columns.Add("TERMINAL_CATEGORY", typeof(String));
            Infos.Columns.Add("HTBM_LOT_NUM", typeof(String));

            Infos.Columns["PRI_KEY"].MaxLength=60;
            Infos.Columns["HEADER_PRI_KEY"].MaxLength=60;
            Infos.Columns["ITEM_NUMBER"].MaxLength=40;
            Infos.Columns["ITEM_DESCRIPTION"].MaxLength=240;
            Infos.Columns["UNIT_OF_MEASURE"].MaxLength=25;
            Infos.Columns["SUBINVENTORY_CODE"].MaxLength=40;
            Infos.Columns["SUBINVENTORY_DESC"].MaxLength=240;
            Infos.Columns["LOT_NUMBER"].MaxLength=60;
            Infos.Columns["BUDGET_PRI_KEY"].MaxLength=60;
            Infos.Columns["HTBM_VENDOR_PRODUCT_ID"].MaxLength=60;
            Infos.Columns["TERMINAL_CATEGORY"].MaxLength=40;
            Infos.Columns["HTBM_LOT_NUM"].MaxLength=60;


            foreach (ImportMaterialReceivingSrvCollection importMaterialReceivingSrvItem in importMaterialReceivingSrvCollection)
            {
                try
                {
                    DataRow Collection = Collections.NewRow();
                    Collection["PRI_KEY"] = importMaterialReceivingSrvItem.PRI_KEY;
                    Collection["BILL_TYPE"] = importMaterialReceivingSrvItem.BILL_TYPE;
                    Collection["IS_DELIVERY"] = importMaterialReceivingSrvItem.IS_DELIVERY;
                    Collection["BILL_CODE"] = importMaterialReceivingSrvItem.BILL_CODE;
                    Collection["CITY_CODE"] = importMaterialReceivingSrvItem.CITY_CODE;
                    Collection["RECEIVER_NUM"] = importMaterialReceivingSrvItem.RECEIVER_NUM;
                    Collection["RECEIVER_DATE"] = importMaterialReceivingSrvItem.RECEIVER_DATE;
                    error = TransformInfo(importMaterialReceivingSrvItem.LineInfo, Infos);
                    if (error.Length!=0)
                        return error;
                    Collections.Rows.Add(Collection);
                    CheckCollection(Collection);
                }
                catch(Exception e)
                {
                    error = "转储Collection失败:"+e.Message;
                    logger.Info("转储Collection失败:"+e.Message);
                    Collections.Rows.Clear();
                    return error;
                }                
            }
            return error;
        }

        private String TransformInfo(List<LineInfo> lineInfo, DataTable Infos)
        {
            String error = "";
            foreach (LineInfo lineInfoItem in lineInfo)
            {
                try
                {
                    DataRow Info = Infos.NewRow();
                    Info["PRI_KEY"] = lineInfoItem.PRI_KEY;
                    Info["HEADER_PRI_KEY"] = lineInfoItem.HEADER_PRI_KEY;
                    Info["ITEM_NUMBER"] = lineInfoItem.ITEM_NUMBER;
                    Info["ITEM_DESCRIPTION"] = lineInfoItem.ITEM_DESCRIPTION;
                    Info["UNIT_OF_MEASURE"] = lineInfoItem.UNIT_OF_MEASURE;
                    Info["SUBINVENTORY_CODE"] = lineInfoItem.SUBINVENTORY_CODE;
                    Info["SUBINVENTORY_DESC"] = lineInfoItem.SUBINVENTORY_DESC;
                    Info["LOT_NUMBER"] = lineInfoItem.LOT_NUMBER;
                    Info["QUANTITY"] = lineInfoItem.QUANTITY;
                    Info["BUDGET_PRI_KEY"] = lineInfoItem.BUDGET_PRI_KEY;
                    Info["HTBM_VENDOR_PRODUCT_ID"] = lineInfoItem.HTBM_VENDOR_PRODUCT_ID;
                    Info["TERMINAL_CATEGORY"] = lineInfoItem.TERMINAL_CATEGORY;
                    Info["HTBM_LOT_NUM"] = lineInfoItem.HTBM_LOT_NUM;
                    Infos.Rows.Add(Info);
                }
                catch (Exception e)
                {
                    error = "转储LineInfo失败:"+e.Message;
                    logger.Info("转储LineInfo失败:" + e.Message);
                    Infos.Rows.Clear();
                    return error;
                }
                
            }
            return error;
        }

        private string CheckCollection(DataRow collection)
        {
            throw new NotImplementedException();
        }

        private string CheckInfo(DataRow info)
        {
            throw new NotImplementedException();
        }

        public string LoadData(DataTable Collections, DataTable Infos)
        {
            string error = "";
            error = importMaterialReceivingDao.LoadCollections(Collections);
            if (error.Length != 0)
            {
                return error;
            }
            error = importMaterialReceivingDao.LoadInfos(Infos);
            if (error.Length != 0)
            {
                return error;
            }
            return error;
        }

        public string CheckData(List<ErrorCollection> errorCollections)
        {
            string error = "";
            error = importMaterialReceivingDao.CheckData(errorCollections);
            if (error.Length != 0)
            {
                return error;
            }
            return error;
        }

        public string EncoderByMd5(String token)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bs = Encoding.UTF8.GetBytes(token);
            byte[] hs = md5.ComputeHash(bs);
            return Convert.ToBase64String(hs);
        }
        public DataTable AddData(DataTable dt1, DataTable dt2)
        {

            //表1结构添加到新表
            DataTable newtable = dt1.Clone();

            //表2结构添加到新表
            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                newtable.Columns.Add(dt2.Columns[i].ColumnName);
            }
            //给新表添数据
            int count = 0;
            object[] value = new object[newtable.Columns.Count];
            if (dt1.Rows.Count > dt2.Rows.Count)
            {
                count = dt1.Rows.Count;
            }
            else
            {
                count = dt2.Rows.Count;
            }

            for (int i = 0; i < count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(value, 0);
                dt2.Rows[i].ItemArray.CopyTo(value, dt1.Columns.Count);
                newtable.Rows.Add(value);
            }

            return newtable;
        }
    }
}