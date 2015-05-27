using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Common;
using Entity;
using Dapper;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Service
{
    public class ConsteService
    {
        #region  获取星座
       // static Dictionary<string, ConstellationEntity> DicCache = new Dictionary<string, ConstellationEntity>();
        private static ConstellationEntity GetConsByApi(string consName)
        {
            ConstellationEntity cons = null;
            cons = DataAccess.ConsConnection()
               .Query<ConstellationEntity>("SELECT * FROM dbo.Conste WHERE Name = @Name and QueryDateTime=@QueryDateTime", new { @Name = consName, @QueryDateTime = DateTime.Now.ToString("yyyy-MM-dd") }).SingleOrDefault<ConstellationEntity>();
            if (cons == null)
            {
                string url = HaoServiceConfig.HaoServiceUrl;
                url += "?key=" + HaoServiceConfig.key;
                url += "&consName=" + consName;
                url += "&type=today";

               
                try
                {
                    string resultapi = HttpHandle.RequestGet(url);

                    var jobj = JsonConvert.DeserializeObject(resultapi) as JObject;
                    if (jobj["reason"].ToString() == "成功")
                    {
                        var jCons = jobj["result"];
                        cons = new ConstellationEntity()
                       {
                           Name = jCons["name"].ToString(),
                           QueryDatetime = jCons["datetime"].ToString(),
                           QueryDate = jCons["date"].ToString(),
                           All = jCons["all"].ToString(),
                           Color = jCons["color"].ToString(),
                           Health = jCons["health"].ToString(),
                           Love = jCons["love"].ToString(),
                           Money = jCons["money"].ToString(),
                           Number = jCons["number"].ToString(),
                           QFriend = jCons["OFriend"].ToString(),
                           Summary = jCons["summary"].ToString(),
                           Work = jCons["work"].ToString()
                       };
                      //  DicCache[consName] = cons;
                        ConsInsert(cons);
                    }
                }
                catch { }
                if (cons == null)
                {
                    //if (DicCache.ContainsKey(consName))
                    //{
                    //    cons = DicCache[consName];
                    //}
                    //else
                    //{
                        cons = GetConsByNameFromDb(consName);
                   // }
                }
            }
            return cons;
            

        }

        public static ConstellationEntity GetCons(string consName)
        {
            ConstellationEntity resultCons = null;
            //if(DicCache.ContainsKey(consName))
            //{
            //    resultCons = DicCache[consName];
            //    if (resultCons.QueryDatetime != DateTime.Now.ToString("yyyy-MM-dd"))
            //    {
                    resultCons = GetConsByApi(consName);
            //    }
            //}
            return resultCons;
        }

        public static ConstellationEntity GetCons(DateTime birthday)
        {
            string consName = GetConsteNameByDate(birthday);
            return GetCons(consName);
        }
        
        /// <summary>
        /// 初始化化所有星座当天运势
        /// </summary>
        public static void  InitAllCons()
        {
            string[] consArray = new string[] { "白羊座", "金牛座", "双子座", "巨蟹座", "狮子座", "处女座", "天秤座", "天蝎座", "射手座", "摩羯座", "水瓶座", "双鱼座" };
            foreach(var item in consArray)
            {
                GetConsByApi(item);
            }
        }

        /// <summary>
        /// 获取星座
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static string GetConsteNameByDate(DateTime birthday)
        {
            float birthdayF = 0.00F;

            if (birthday.Month == 1 && birthday.Day < 20)
            {
                if (birthday.Day < 10)
                    birthdayF = float.Parse(string.Format("13.0{0}", birthday.Day));
                else
                    birthdayF = float.Parse(string.Format("13.{0}", birthday.Day));
            }
            else
            {
                if (birthday.Day < 10)
                    birthdayF = float.Parse(string.Format("{0}.0{1}", birthday.Month, birthday.Day));
                else
                    birthdayF = float.Parse(string.Format("{0}.{1}", birthday.Month, birthday.Day));
            }

            float[] atomBound = { 1.20F, 2.20F, 3.21F, 4.21F, 5.21F, 6.22F, 7.23F, 8.23F, 9.23F, 10.23F, 11.21F, 12.22F, 13.20F };
            string[] atoms = { "水瓶座", "双鱼座", "白羊座", "金牛座", "双子座", "巨蟹座", "狮子座", "处女座", "天秤座", "天蝎座", "射手座", "魔羯座" };

            string ret = "";
            for (int i = 0; i < atomBound.Length - 1; i++)
            {
                if (atomBound[i] <= birthdayF && atomBound[i + 1] > birthdayF)
                {
                    ret = atoms[i];
                    break;
                }
            }
            return ret;
        }

        public static int ConsInsert(ConstellationEntity cons)
        {
            string sql = "INSERT INTO [dbo].[Conste]([Name] ,[QueryDateTime],[QueryDate],[All],[Color] ,[Health] ,[Love],[Money],[Number] ,[QFriend],[Summary] ,[Work],[CreateTime])VALUES(@Name ,@QueryDateTime,@QueryDate,@All ,@Color ,@Health,@Love ,@Money,@Number,@QFriend ,@Summary,@Work,@CreateTime)";
            return DataAccess.ConsConnection()
                .Execute(sql, new { @Name=cons.Name,@QueryDateTime=cons.QueryDatetime,@QueryDate=cons.QueryDate,@All=cons.All,@Color=cons.Color,@Health=cons.Health,@Love=cons.Love,@Money=cons.Money,@Number=cons.Number,@QFriend=cons.QFriend,@Summary=cons.Summary,@Work=cons.Work, @CreateTime=DateTime.Now});
        }

        public static ConstellationEntity GetConsByNameFromDb(string name)
        {
            string sql = "select top 1 * from [dbo].[Conste] where Name= @Name order by createTime  desc";
            var result = DataAccess.ConsConnection()
                .Query<ConstellationEntity>(sql, new { @Name = name }).FirstOrDefault<ConstellationEntity>();
            return result;
        }
        #endregion

        #region 获取用户
        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <returns></returns>
        public static UserConsEntity GetUserConsByUserId(string userID)
        {
            var result = DataAccess.ConsConnection()
                .Query<UserConsEntity>("SELECT * FROM dbo.ConUser WHERE UserID = @UserID and consName is not null order by desc", new { @UserID = userID }).SingleOrDefault<UserConsEntity>();
            if (result == null)
            {
                string url = WebServiceConfig.user_get_url;
                url += "?access_token=" + HttpContext.Current.Request.Cookies.Get("access_token").Value;
                //url += "?access_token=" + accessToken;
                url += "&u_id=" + userID;
                url += "&format=" + WebServiceConfig.format;

                string resultUser = HttpHandle.RequestGet(url);

                var jUser = (JsonConvert.DeserializeObject(resultUser) as JObject)["user"];
                result = new UserConsEntity();
                
                result.ImgUrl = jUser["avatar100"].ToString();
                result.UserID = new Guid(jUser["id"].ToString());
                result.UserName = jUser["name"].ToString();
                result.BirthDay = jUser["birth"].ToString();
                result.Sex = jUser["gender"].ToString();
                result.Age=0;
                    DateTime bd = DateTime.Now;
                bool isDateTime = DateTime.TryParse(result.BirthDay,out bd);
                if(isDateTime)
                {
                    result.Age = DateTime.Now.Year - DateTime.Parse(jUser["birth"].ToString()).Year;
                    result.ConsName = GetConsteNameByDate(DateTime.Parse(jUser["birth"].ToString()));
                }
                UserConsInsert(result);
              };
                
            return result;
        }

        public static List<UserConsEntity> GetUserConsByConsName(string consName)
        {
            var result = DataAccess.ConsConnection()
                .Query<UserConsEntity>("SELECT * FROM dbo.ConUser WHERE ConsName = @ConsName", new { @ConsName = consName }).ToList<UserConsEntity>();
            return result;
        }

        public static List<UserConsEntity> GetUserConsByConsNameSex(string consName, string sex)
        {
            var result = DataAccess.ConsConnection()
                .Query<UserConsEntity>("SELECT * FROM dbo.ConUser WHERE ConsName = @ConsName And Sex=@Sex", new { @ConsName = consName, @Sex=sex }).ToList<UserConsEntity>();
            return result;
        }

        public static void InitAllUserCons()
        {
            string url = HaoServiceConfig.user_all_send_url;
            url += "?access_token=" + HttpContext.Current.Request.Cookies.Get("access_token").Value;
            //url += "&pageindex=1";
            //url += "&pagesize=3000";
            url += "&format=" + WebServiceConfig.format;

            string resultUser = HttpHandle.RequestGet(url);
            
            var jUser = (JArray)(JsonConvert.DeserializeObject(resultUser) as JObject)["users"];
            foreach (var item in jUser)
            {
                string userId = item["id"].ToString();
                GetUserConsByUserId(userId);
            }
           
        }

        public static void GetUserDetail()
        {
            string getUserResult = HttpHandle.RequestGet(string.Format("{0}?access_token={1}&format={2}", WebServiceConfig.getUser_send_url, HttpContext.Current.Request.Cookies.Get("access_token").Value, WebServiceConfig.format));
            WebUserInfo webUserInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WebUserInfo>(getUserResult);
            //用户信息保存到cookie
            user user = webUserInfo.user;
        
            UserConsEntity userNew = new UserConsEntity();
            string userid = "";
     
            userNew.UserID = new Guid(user.id);
            userNew.UserName = user.name;
            userNew.ImgUrl = user.avstar100;
            userNew.BirthDay = user.birth;
            userNew.Age = 0;
            userNew.Sex = user.gender;
            userNew.ConsName = "";
            DateTime result1 = DateTime.Now;
            bool isDateTime = DateTime.TryParse(user.birth, out result1);
            if(isDateTime)
            {
                userNew.BirthDay = user.birth;
                userNew.Age = DateTime.Now.Year - result1.Year;
                userNew.ConsName = ConsteService.GetConsteNameByDate(result1);
            }
            UserConsInsert(userNew);

        }

        public static int UserConsInsert(UserConsEntity user)
        {
            return DataAccess.ConsConnection()
                .Execute("INSERT INTO dbo.ConUser ( UserID, UserName, ImgUrl,BirthDay,Sex,Age,ConsName ) VALUES  ( @UserID,@UserName,@ImgUrl,@BirthDay,@Sex,@Age,@ConsName)"
                , new { @UserID = user.UserID, @UserName = user.UserName, @ImgUrl = user.ImgUrl,@BirthDay=user.BirthDay,@Sex=user.Sex,@Age=user.Age,@ConsName=user.ConsName });
        }

        /// <summary>
        /// 福星高照
        /// </summary>
        /// <returns></returns>
        public static UserConsEntity GetGoodLuckUser()
        {
            string sql = "select A.* from dbo.conste A, (select max(cast(queryDateTime as datetime)) as maxtime,name from Conste group by name) B where cast(A.queryDateTime as datetime) = maxtime and A.name = B.name";
            var listCons = DataAccess.ConsConnection()
               .Query<ConstellationEntity>(sql,null);
            ConstellationEntity maxItem = listCons.OrderByDescending(m => int.Parse(m.All.Trim('%'))).First<ConstellationEntity>();
            string consName = maxItem.Name;

            string userSql = string.Format("select * from dbo.conuser where consName='{0}' and sex='2' ", consName);
            var listUsers = DataAccess.ConsConnection()
               .Query<UserConsEntity>(userSql, null);


            UserConsEntity result = listUsers.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// 献出一点爱用户
        /// </summary>
        /// <param name="consName"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        public static UserConsEntity GetMissUser()
        {
            string sql = "select A.* from dbo.conste A, (select max(cast(queryDateTime as datetime)) as maxtime,name from Conste group by name) B where cast(A.queryDateTime as datetime) = maxtime and A.name = B.name";
            var listCons = DataAccess.ConsConnection()
               .Query<ConstellationEntity>(sql, null);
            ConstellationEntity minItem = listCons.OrderBy(m => int.Parse(m.All.Trim('%'))).First<ConstellationEntity>();
            string consName = minItem.Name;

            string userSql = string.Format("select * from dbo.conuser where consName='{0}' and sex='1' ", consName);
            var listUsers = DataAccess.ConsConnection()
               .Query<UserConsEntity>(userSql, null);
            UserConsEntity result = listUsers.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// 献出一点爱用户
        /// </summary>
        /// <param name="consName"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        public static UserConsEntity GetLoveUser(UserConsEntity currentUser)
        {
            string consName = currentUser.ConsName;
            var consEntity = GetConsByNameFromDb(consName);
            var QFriendConsName = consEntity.QFriend;
            string userSql = string.Empty;
            string sexarg = "2";
            if (currentUser.Sex == "1")
            {
                sexarg = "2";
            }
            else if(currentUser.Sex == "2")
            {
                sexarg = "1";
            }
            else if(currentUser.Sex == "0")
            {
                sexarg = "0";
            }
            userSql = string.Format("select * from dbo.conuser where consName='{0}' and sex='{1}' ", QFriendConsName, sexarg);
            var listUsers = DataAccess.ConsConnection()
               .Query<UserConsEntity>(userSql, null);
            UserConsEntity result = listUsers.FirstOrDefault();

            return result;
        }

        public static string GetUserList(string param)
        {
            string url = HaoServiceConfig.user_search_send_url;
            url += "?access_token=" + HttpContext.Current.Request.Cookies.Get("access_token").Value;
            ///  url += "?access_token="+
            url += "&keywords=" + param;
            url += "&pageindex=1";
            url += "&pagesize=20";
            url += "&format=" + HaoServiceConfig.format;

            string resultUser = HttpHandle.RequestGet(url);

            return resultUser;

        }

        #endregion

        #region 获取ViewModel
        public static ViewModelCons GetViewModelByUserId(string userId)
        {
            var result = new ViewModelCons();
            result.resultFlag = "0";
            try
            {
                UserConsEntity userCons = GetUserConsByUserId(userId);
                if (userCons != null)
                {

                    result.CurrentUserID = userCons.UserID;
                    result.CurrentUserConsEntity = userCons;
                    result.CurrentConsteEntity = GetCons(userCons.ConsName);
                    result.resultFlag = "1";
                }
                result.CurrentUserConsEntity = userCons;
                result.GoodLuckUserConsEntity = GetGoodLuckUser();
                result.LoveUserConsEntity = GetLoveUser(userCons);
                result.MissUserConsEntity = GetMissUser();
            }
            catch { }
            return result;
        }

        //public static InitToken()
        //{
        //     string code = Request["code"].ToString();
        //string url = "";
        //url += "app_key=" + HaoServiceConfig.app_Key;
        //url += "&app_secret=" + HaoServiceConfig.app_Secret;
        //url += "&grant_type=" + HaoServiceConfig.grant_type;
        //url += "&code=" + code;
        //url += "&redirect_uri=" + HaoServiceConfig.authorize_redirect_uri;
        //url += "&format=" + HaoServiceConfig.format;
        //string result = HttpHandle.RequestPost(HaoServiceConfig.getaccesstoken_send_url, url);
        //GetAccessToken getAccessToken = Newtonsoft.Json.JsonConvert.DeserializeObject<GetAccessToken>(result);
        ////保存accesstoken
        ////WebServiceConfig.access_token = getAccessToken.access_token;
        //HttpCookie tokenCookie = new HttpCookie("access_token");
        //tokenCookie.Value = getAccessToken.access_token;
        //Response.Cookies.Set(tokenCookie);
        //}

        /// <summary>
        /// 发私聊
        /// </summary>
        /// <param name="toUserId"></param>
        /// <param name="msg"></param>
        public static void SendPrivateMsg(string toUserId, string msg)
        {
            string url = HaoServiceConfig.user_search_send_url;
            url += "?access_token=" + HttpContext.Current.Request.Cookies.Get("access_token").Value;
            url += "&u_id=" + toUserId;
            url += "&msg="+msg;
            url += "&format=" + HaoServiceConfig.format;
            HttpHandle.RequestPost(url,url);
        }

        /// <summary>
        /// 发动态给自己
        /// </summary>
        /// <param name="meUserId"></param>
        /// <param name="msg"></param>
        public static void ShowMeMsg(string meUserId, string msg)
        {
            string url = HaoServiceConfig.user_search_send_url;
            url += "?access_token=" + HttpContext.Current.Request.Cookies.Get("access_token").Value;
            if (!string.IsNullOrEmpty(meUserId))
            {
                url += "&g_id=" + meUserId;
            }
            url += "&p_msg=" + msg;
            url += "&format=" + HaoServiceConfig.format;
            HttpHandle.RequestPost(url, url);
        }
        #endregion


        ////未选
        //Default = 0,
        ////男性
        //Mail = 1,
        ////女性
        //Femail = 2
    }
}
