using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using XF.Api.Lib;
using XFramework.Entity;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DefaultClient client = new DefaultClient("UserApi");

        UserRequest userReq = new UserRequest();

        userReq.userName = "123456788";

        SingleResult<string> rtn = client.Execute(userReq);

        Response.Write(rtn.Data);
    }

    public class UserRequest : XFRequest<SingleResult<string>>
    {
        public string userName { get; set; }

        public override string GetApiMethod()
        {
            return "GetUserName";
        }
    }
}