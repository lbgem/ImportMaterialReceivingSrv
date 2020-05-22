using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportMaterialReceivingSrv
{
    public class ErrorCollection
    {
        String _ERROR_FLAG;
        String _ERROR_PRI_KEY;
        String _ERROR_MESSAGE;
        String _RESERVED_1;
        String _RESERVED_2;

        public string ERROR_FLAG { get => _ERROR_FLAG; set => _ERROR_FLAG = value; }
        public string ERROR_PRI_KEY { get => _ERROR_PRI_KEY; set => _ERROR_PRI_KEY = value; }
        public string ERROR_MESSAGE { get => _ERROR_MESSAGE; set => _ERROR_MESSAGE = value; }
        public string RESERVED_1 { get => _RESERVED_1; set => _RESERVED_1 = value; }
        public string RESERVED_2 { get => _RESERVED_2; set => _RESERVED_2 = value; }
    }
}