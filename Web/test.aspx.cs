using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Service;
using Entity;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class test : System.Web.UI.Page
{
    private Dictionary<string, ConstellationEntity> DicCache = new Dictionary<string, ConstellationEntity>();
    protected void Page_Load(object sender, EventArgs e)
    {
        //UserEntity user = UserService.GetUserByID("0392E8F4-685B-4F0C-9D21-9E054D587F50");
        //Dictionary<string, object> json = UserService.GetUserMsg("0392E8F4-685B-4F0C-9D21-9E054D587F50", Convert.ToDateTime("2014-09-01"), Convert.ToDateTime("2014-09-03"));
       // string userList = UserService.GetUserList("袁帅");

        //Response.Write(userList);
        //Response.End();
        //var consName=ConsteService.GetConsteByDate(DateTime.Now);
        //Response.Write(consName);
        //var apistr = ConsteService.GetDetailByConsNameByApi(consName);
        
       // Response.End();
    }
}