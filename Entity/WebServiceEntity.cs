using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class GetAccessToken
    {
        public string access_token { get; set; }

        public string expires_in { get; set; }

        public string refresh_token { get; set; }

    }

    public class WebUserInfo
    {
        public user user { get; set; }

    }

    public class user
    {
        public string id { get; set; }
        public string name { get; set; }
        public string avstar100 { get; set; }
    }

}
