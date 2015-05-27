using Common;
using Entity;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class loginredirect : System.Web.UI.Page
{
    /// <summary>
    /// 获取授权过的令牌
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e) 
    {
        string code = Request["code"].ToString();
        string url = "";
        url += "app_key="+HaoServiceConfig.app_Key;
        url += "&app_secret=" + HaoServiceConfig.app_Secret;
        url += "&grant_type=" + HaoServiceConfig.grant_type;
        url += "&code=" + code;
        url += "&redirect_uri=" + HaoServiceConfig.authorize_redirect_uri;
        url += "&format=" + HaoServiceConfig.format;
        string result = HttpHandle.RequestPost(HaoServiceConfig.getaccesstoken_send_url, url);
        GetAccessToken getAccessToken = Newtonsoft.Json.JsonConvert.DeserializeObject<GetAccessToken>(result);
        //保存accesstoken
        //WebServiceConfig.access_token = getAccessToken.access_token;
        HttpCookie tokenCookie = new HttpCookie("access_token");
        tokenCookie.Value = getAccessToken.access_token;
        Response.Cookies.Set(tokenCookie);
        //获取用户信息

        string getUserResult = HttpHandle.RequestGet(string.Format("{0}?access_token={1}&format={2}", WebServiceConfig.getUser_send_url, getAccessToken.access_token, WebServiceConfig.format));
        //Response.Write(getUserResult);
        WebUserInfo webUserInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WebUserInfo>(getUserResult);
        //用户信息保存到cookie
        user user = webUserInfo.user;
        HttpCookie cookAll = Request.Cookies["user"];
        UserEntity userNew = new UserEntity();
        string userid = "";
        if (cookAll == null)
        {
            HttpCookie cook = new HttpCookie("user");
            userNew.UserID = new Guid(user.id);
            userNew.UserName = user.name;
            userNew.ImgUrl = user.avstar100;
            cook.Value = HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(userNew));
            Response.Cookies.Add(cook);
            userid = user.id;
        }
        //else
        //{
        //    userid = UserService.GetUser().UserID.ToString();
        //}
        ////查询用户是否在数据库中，若是不在，则新增
        //UserEntity userEntity = UserService.GetUserByID(userid);
        //if (userEntity == null)
        //{
        //    //调用新增方法，添加用户
        //    UserEntity paramUser = new UserEntity();
        //    paramUser.UserID = new Guid(user.id);
        //    paramUser.UserName = user.name;
        //    paramUser.ImgUrl = user.avstar100;
        //    UserService.UserInsert(paramUser);
        //}
        //跳转到首页
        Response.Redirect("lover.html");

    }
}