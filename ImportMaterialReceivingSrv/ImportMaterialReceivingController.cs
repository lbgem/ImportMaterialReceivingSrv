using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace ImportMaterialReceivingSrv
{
    public class ImportMaterialReceivingController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ImportMaterialReceivingController));
        public ImportMaterialReceivingSrvResponse Read(ImportMaterialReceivingSrvRequest importMaterialReceivingSrvRequest)
        {
            ImportMaterialReceivingService importMaterialReceivingService = new ImportMaterialReceivingService();
            ImportMaterialReceivingSrvResponse importMaterialReceivingSrvResponse = new ImportMaterialReceivingSrvResponse();
            MsgHeader msgHeader = new MsgHeader();
            List<ImportMaterialReceivingSrvCollection> importMaterialReceivingSrvCollection = new List<ImportMaterialReceivingSrvCollection>();
            List<ErrorCollection> errorCollections = new List<ErrorCollection>();
            String error = "";
            DataTable Collections = new DataTable("Collections");
            DataTable Infos = new DataTable("Infos");
            DataTable ErrorCollection = new DataTable("ErrorCollection");
            msgHeader =importMaterialReceivingService.GetMsgheader(importMaterialReceivingSrvRequest);
            error=importMaterialReceivingService.CheckMsgheader(msgHeader);
            importMaterialReceivingSrvResponse.ERROR_FLAG = "Y";
            if (error.Length!=0)
            {
                return FastResponse(error);
            }
            importMaterialReceivingSrvCollection = importMaterialReceivingService.GetCollection(importMaterialReceivingSrvRequest);
            error = importMaterialReceivingService.CheckCollection(importMaterialReceivingSrvCollection);
            if (error.Length != 0)
            {
                return FastResponse(error);
            }
            error = importMaterialReceivingService.TransformData(msgHeader, importMaterialReceivingSrvCollection, Collections, Infos);
            if (error.Length!=0)
            {
                return FastResponse(error);
            }
            error = importMaterialReceivingService.LoadData(Collections, Infos);
            if (error.Length != 0)
            {
                return FastResponse(error);
            }
            error = importMaterialReceivingService.CheckData(errorCollections);
            return importMaterialReceivingSrvResponse;
        }
        public ImportMaterialReceivingSrvResponse FastResponse(String error)
        {
            ImportMaterialReceivingSrvResponse SrvResponse = new ImportMaterialReceivingSrvResponse();
            SrvResponse.ERROR_FLAG = "N";
            SrvResponse.ERROR_PRI_KEY = DateTime.Now.ToString("yyyyMMddHHmmssffff", DateTimeFormatInfo.InvariantInfo);
            SrvResponse.ERROR_RETURN_MESSAGE = error;
            return SrvResponse;
        }

    }
}