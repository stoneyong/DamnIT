using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common
{
    public class WebServiceConfig
    {
        public static string app_Key = "E4159ACC0CCA";

        public static string app_Secret = "B45B274019878997BE3A9722125E94A4";

        public static string authorize_send_uri = "https://api.mingdao.com/oauth2/authorize";

        public static string authorize_redirect_uri = "http://localhost:31795/loginredirect.aspx";

        public static string getaccesstoken_send_url = "https://api.mingdao.com/oauth2/access_token";

        public static string grant_type = "authorization_code";

        public static string format = "json";

        public static string access_token = "";

        public static string getUser_send_url = "https://api.mingdao.com/passport/detail";

        public static string user_get_url = "https://api.mingdao.com/user/detail";

        public static string user_search_send_url = "https://api.mingdao.com/user/search";

    }
}