using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportMaterialReceivingSrv
{
    public class ImportMaterialReceivingSrvCollection
    {
        String _PRI_KEY;
        String _BILL_TYPE;
        String _IS_DELIVERY;
        String _BILL_CODE;
        String _CITY_CODE;
        String _RECEIVER_NUM;
        DateTime _RECEIVER_DATE;
        List<LineInfo> _lineInfo;

        public string PRI_KEY { get => _PRI_KEY; set => _PRI_KEY = value; }
        public string BILL_TYPE { get => _BILL_TYPE; set => _BILL_TYPE = value; }
        public string IS_DELIVERY { get => _IS_DELIVERY; set => _IS_DELIVERY = value; }
        public string BILL_CODE { get => _BILL_CODE; set => _BILL_CODE = value; }
        public string CITY_CODE { get => _CITY_CODE; set => _CITY_CODE = value; }
        public string RECEIVER_NUM { get => _RECEIVER_NUM; set => _RECEIVER_NUM = value; }
        public DateTime RECEIVER_DATE { get => _RECEIVER_DATE; set => _RECEIVER_DATE = value; }
        public List<LineInfo> LineInfo { get => _lineInfo; set => _lineInfo = value; }
    }
}