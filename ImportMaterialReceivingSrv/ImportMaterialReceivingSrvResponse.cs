using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportMaterialReceivingSrv
{
    public class ImportMaterialReceivingSrvResponse
    {
        String _ERROR_FLAG;
        String _ERROR_PRI_KEY;
        String _ERROR_RETURN_MESSAGE;
        List<ErrorCollection> _errorCollection;

        public string ERROR_FLAG { get => _ERROR_FLAG; set => _ERROR_FLAG = value; }
        public string ERROR_PRI_KEY { get => _ERROR_PRI_KEY; set => _ERROR_PRI_KEY = value; }
        public string ERROR_RETURN_MESSAGE { get => _ERROR_RETURN_MESSAGE; set => _ERROR_RETURN_MESSAGE = value; }
        public List<ErrorCollection> ErrorCollection { get => _errorCollection; set => _errorCollection = value; }
    }
}