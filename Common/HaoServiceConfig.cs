using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class HaoServiceConfig
    {
  
            //http://apis.haoservice.com/lifeservice/constellation/GetAll?consName=白羊座&type=today&key=您申请的APPKEY
        public static string HaoServiceUrl = @"http://apis.haoservice.com/lifeservice/constellation/GetAll";
        public static string key = "a6e9706ccf7f4d2983a7e02a0f112042";


        ///https://api.mingdao.com/oauth2/authorize?app_key=71A9858BA701&redirect_uri=http://localhost:31795/loginredirect.aspx
        ///


        public static string app_Key = "71A9858BA701";
        public static string app_Secret = "C99526DB1819C968E67CEE56F01AAD";

        public static string authorize_send_uri = "https://api.mingdao.com/oauth2/authorize";

        public static string authorize_redirect_uri = "http://localhost:31795/login.aspx";

        public static string getaccesstoken_send_url = "https://api.mingdao.com/oauth2/access_token";

        public static string grant_type = "authorization_code";

        public static string format = "json";

        public static string access_token = "";

        public static string getUser_send_url = "https://api.mingdao.com/passport/detail";

        public static string user_get_url = "https://api.mingdao.com/user/detail";

        public static string user_search_send_url = "https://api.mingdao.com/user/search";

        public static string user_all_send_url = "https://api.mingdao.com/user/all";

        public static string user_sendmsg_url = "https://api.mingdao.com/message/create";

        public static string user_showme_url = "https://api.mingdao.com/post/update";

    }
}
