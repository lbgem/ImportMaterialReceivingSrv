using System.Collections.Generic;

namespace ImportMaterialReceivingSrv
{
    public class ImportMaterialReceivingSrvRequest
    {
        MsgHeader _msgHeader;
        List<ImportMaterialReceivingSrvCollection> _srvCollection;

        public MsgHeader MsgHeader { get => _msgHeader; set => _msgHeader = value; }
        public List<ImportMaterialReceivingSrvCollection> SrvCollection { get => _srvCollection; set => _srvCollection = value; }
    }
}