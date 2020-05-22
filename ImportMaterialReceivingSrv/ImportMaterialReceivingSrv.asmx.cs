using log4net;
using System.Web.Services;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace ImportMaterialReceivingSrv
{
    /// <summary>
    /// ImportMaterialReceivingSrv 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class ImportMaterialReceivingSrv : System.Web.Services.WebService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ImportMaterialReceivingSrv));
        [WebMethod]
        public ImportMaterialReceivingSrvResponse process(ImportMaterialReceivingSrvRequest importMaterialReceivingSrvRequest)
        {
            logger.Info("\r\n");
            logger.Info("WebsSrvice Start......");
            ImportMaterialReceivingSrvResponse importMaterialReceivingSrvResponse = new ImportMaterialReceivingSrvResponse();
            ImportMaterialReceivingController importMaterialReceivingController = new ImportMaterialReceivingController();
            importMaterialReceivingSrvResponse = importMaterialReceivingController.Read(importMaterialReceivingSrvRequest);
            logger.Info(importMaterialReceivingSrvResponse);
            logger.Info("WebsSrvice End......");
            return importMaterialReceivingSrvResponse;
        }
    }
}
