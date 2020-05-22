using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ImportMaterialReceivingSrv
{
    public class MsgHeader
    {     
        String _SOURCESYSTEMID;
        String _SOURCESYSTEMNAME;
        String _TOKEN;
        int? _USER_ID;
        String _USER_NAME;
        String _USER_PASSWD;
        DateTime? _SUBMIT_DATE;
        int? _PAGE_SIZE;
        int? _CURRENT_PAGE;
        int? _TOTAL_RECORD;
        String _RESERVED_1;
        String _RESERVED_2;
        public string SOURCESYSTEMID { get => _SOURCESYSTEMID; set => _SOURCESYSTEMID = value; }
        public string SOURCESYSTEMNAME { get => _SOURCESYSTEMNAME; set => _SOURCESYSTEMNAME = value; }
        public string TOKEN { get => _TOKEN; set => _TOKEN = value; }
        [XmlElement(IsNullable = true)]
        public int? USER_ID { get => _USER_ID; set => _USER_ID = value; }
        [XmlElement(IsNullable = true)]
        public string USER_NAME { get => _USER_NAME; set => _USER_NAME = value; }
        [XmlElement(IsNullable = true)]
        public string USER_PASSWD { get => _USER_PASSWD; set => _USER_PASSWD = value; }
        [XmlElement(IsNullable = true)]
        public DateTime? SUBMIT_DATE { get => _SUBMIT_DATE; set => _SUBMIT_DATE = value; }
        [XmlElement(IsNullable = true)]
        public int? PAGE_SIZE { get => _PAGE_SIZE; set => _PAGE_SIZE = value; }
        [XmlElement(IsNullable = true)]
        public int? CURRENT_PAGE { get => _CURRENT_PAGE; set => _CURRENT_PAGE = value; }
        [XmlElement(IsNullable = true)]
        public int? TOTAL_RECORD { get => _TOTAL_RECORD; set => _TOTAL_RECORD = value; }
        [XmlElement(IsNullable = true)]
        public string RESERVED_1 { get => _RESERVED_1; set => _RESERVED_1 = value; }
        [XmlElement(IsNullable = true)]
        public string RESERVED_2 { get => _RESERVED_2; set => _RESERVED_2 = value; }
    }
}