using Common;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Redirect(string.Format("{0}?app_key={1}&redirect_uri={2}&response_type=code", WebServiceConfig.authorize_send_uri, WebServiceConfig.app_Key, WebServiceConfig.authorize_redirect_uri));
    }
}