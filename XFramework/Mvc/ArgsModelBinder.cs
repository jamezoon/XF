using System.IO;
using System.Web;
using System.Web.Mvc;

namespace XFramework.Mvc
{
    public class ArgsModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext context, ModelBindingContext bindingContext)
        {
            string rtnRst = string.Empty;

            HttpRequestBase request = context.HttpContext.Request;

            if (request.HttpMethod == "POST")
            {
                request.InputStream.Position = 0;

                using (StreamReader stream = new StreamReader(request.InputStream))
                {
                    rtnRst = stream.ReadToEnd();
                }
            }
            else
            {
                rtnRst = request.QueryString["data"];
            }

            return rtnRst;
        }
    }
}
