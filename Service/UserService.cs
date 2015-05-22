using Dapper;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Service
{
    public class UserService
    {
        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        /// <returns></returns>
        public static UserEntity GetUser()
        {
            UserEntity user = new UserEntity();
            HttpCookie cookUser = HttpContext.Current.Request.Cookies["user"];
            user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserEntity>(HttpUtility.UrlDecode(cookUser.Value));
            //获取今日评价次数
            user.TodayCount = GetTodayCount(user.UserID.ToString());
            return user;
        }
        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <returns></returns>
        public static UserEntity GetUserByID(string userID)
        {
            var result = DataAccess.OpenConnection()
                .Query<UserEntity>("SELECT * FROM dbo.T_User WHERE UserID = @UserID", new { @UserID = userID }).SingleOrDefault<UserEntity>();
            if (result == null)
            {
                string url = WebServiceConfig.user_get_url;
                url += "?access_token=" + HttpContext.Current.Request.Cookies.Get("access_token").Value;
                url += "&u_id=" + userID;
                url += "&format=" + WebServiceConfig.format;

                string resultUser = HttpHandle.RequestGet(url);

                var jUser  = (JsonConvert.DeserializeObject(resultUser) as JObject)["user"];
                result = new UserEntity()
                {
                    ImgUrl = jUser["avatar100"].ToString(),
                    TodayCount = 0,
                    UserID = new Guid(jUser["id"].ToString()),
                    UserName = jUser["name"].ToString()
                };
            }
            return result;
        }
        /// <summary>
        /// 获取一段时间，某用户的统计信息以及被评价详情信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetUserMsg(string userID, DateTime beginDate, DateTime endDate)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            //获取用户基本信息
            //UserEntity user = GetUserByID(userID);
            //jsonResult.Add("UserID", user.UserID);
            //jsonResult.Add("UserName", user.UserName);
            //jsonResult.Add("ImgUrl", user.ImgUrl);
            List<AppraiseEntity> appraiseList = AppraiseService.GetAppraise(new Guid(userID), beginDate, endDate);
            if (appraiseList.Count > 0)
            {
                jsonResult.Add("WorkloadNum", appraiseList.Where(x => (x.AppraiseClass == 1 && x.AppraiseType == 2)).ToList<AppraiseEntity>().Count);//评价该员工工作量人数
                jsonResult.Add("WorkEfficiencyNum", appraiseList.Where(x => (x.AppraiseClass == 2 && x.AppraiseType == 2)).ToList<AppraiseEntity>().Count);//工作效率
                jsonResult.Add("CooperationNum", appraiseList.Where(x => (x.AppraiseClass == 3 && x.AppraiseType == 2)).ToList<AppraiseEntity>().Count); //合作性
                //自我评价
                int mySelfCount = appraiseList.Where(x => x.AppraiseType == 1).ToList<AppraiseEntity>().Count;
                decimal sumWorkloadSelf = 0;
                decimal sumWorkEfficiencySelf = 0;
                decimal sumCooperationSelf = 0;
                int i = 0;
                if (mySelfCount > 0)
                {
                    Dictionary<string, object>[] jsonSelfList = new Dictionary<string, object>[mySelfCount];
                    List<AppraiseEntity> aeList = appraiseList.Where(x => x.AppraiseType == 1).ToList<AppraiseEntity>();
                    foreach (AppraiseEntity appraiseItem in aeList)
                    {
                        Dictionary<string, object> jsonItem = new Dictionary<string, object>();
                        jsonItem.Add("AppraiseDate", appraiseItem.AppraiseDate);
                        switch (appraiseItem.AppraiseClass)
                        {
                            case 1:
                                jsonItem.Add("WorkloadScore", appraiseItem.Score);
                                sumWorkloadSelf += appraiseItem.Score;
                                break;
                            case 2:
                                jsonItem.Add("WorkEfficiencyScore", appraiseItem.Score);
                                sumWorkEfficiencySelf += appraiseItem.Score;
                                break;
                            case 3:
                                jsonItem.Add("CooperationScore", appraiseItem.Score);
                                sumCooperationSelf += appraiseItem.Score;
                                break;
                            default:
                                break;
                        }
                        jsonSelfList.SetValue(jsonItem, i);
                        i++;
                    }
                    jsonResult.Add("SelfList", jsonSelfList);
                }
                int othersCount = appraiseList.Where(x => x.AppraiseType == 2).ToList<AppraiseEntity>().Count;
                //他人评价
                decimal sumWorkload = 0;
                decimal sumWorkEfficiency = 0;
                decimal sumCooperation = 0;
                if (othersCount > 0)
                {
                    Dictionary<string, object>[] jsonOthersList = new Dictionary<string, object>[othersCount];
                    i = 0;
                    List<AppraiseEntity> appList = appraiseList.Where(x => x.AppraiseType == 2).ToList<AppraiseEntity>();
                    foreach (AppraiseEntity appraiseItem in appList)
                    {
                        Dictionary<string, object> jsonItem = new Dictionary<string, object>();
                        jsonItem.Add("AppraiseDate", appraiseItem.AppraiseDate);
                        switch (appraiseItem.AppraiseClass)
                        {
                            case 1:
                                jsonItem.Add("WorkloadScore", appraiseItem.Score);
                                sumWorkload += appraiseItem.Score;
                                break;
                            case 2:
                                jsonItem.Add("WorkEfficiencyScore", appraiseItem.Score);
                                sumWorkEfficiency += appraiseItem.Score;
                                break;
                            case 3:
                                jsonItem.Add("CooperationScore", appraiseItem.Score);
                                sumCooperation += appraiseItem.Score;
                                break;
                            default:
                                break;
                        }
                        jsonOthersList.SetValue(jsonItem, i);
                        i++;
                    }
                    jsonResult.Add("OthersList", jsonOthersList);
                    //别人给我的得分
                    jsonResult.Add("SumWorkload", sumWorkload);
                    jsonResult.Add("SumWorkEfficiency", sumWorkEfficiency);
                    jsonResult.Add("SumCooperation", sumCooperation);
                    jsonResult.Add("DayCount", (endDate-beginDate).TotalDays);
                }


                //计算自我评价平均得分
                decimal workloadScoreSelf = 0;
                decimal WorkEfficiencyScoreSelf = 0;
                decimal CooperationScoreSelf = 0;

                //计算别人给我的评价的平均得分
                decimal workloadScoreOthers = 0;
                decimal WorkEfficiencyScoreOthers = 0;
                decimal CooperationScoreOthers = 0;
                //if (jsonResult.ContainsKey("WorkEfficiencyNum"))
                //{
                workloadScoreSelf = Convert.ToDecimal(GetRound(Convert.ToDouble(sumWorkloadSelf), Convert.ToInt32(jsonResult["WorkloadNum"])));
                WorkEfficiencyScoreSelf = Convert.ToDecimal(GetRound(Convert.ToDouble(sumWorkEfficiencySelf), Convert.ToInt32(jsonResult["WorkEfficiencyNum"])));
                CooperationScoreSelf = Convert.ToDecimal(GetRound(Convert.ToDouble(sumCooperationSelf), Convert.ToInt32(jsonResult["CooperationNum"])));
                if (jsonResult.ContainsKey("SumWorkload"))
                {
                    workloadScoreOthers = Convert.ToDecimal(GetRound(Convert.ToDouble(jsonResult["SumWorkload"]), Convert.ToInt32(jsonResult["WorkloadNum"])));
                    WorkEfficiencyScoreOthers = Convert.ToDecimal(GetRound(Convert.ToDouble(jsonResult["SumWorkEfficiency"]), Convert.ToInt32(jsonResult["WorkEfficiencyNum"])));
                    CooperationScoreOthers = Convert.ToDecimal(GetRound(Convert.ToDouble(jsonResult["SumCooperation"]), Convert.ToInt32(jsonResult["CooperationNum"])));
                }
                //}


                //判断该亮那个灯
                if (workloadScoreSelf > (workloadScoreOthers * Convert.ToDecimal(1.2)))
                {
                    jsonResult.Add("Lamp", "damn it");
                }
                else if (workloadScoreSelf > (workloadScoreOthers * Convert.ToDecimal(1.1)))
                {
                    jsonResult.Add("Lamp", "warning");
                }
                else
                {
                    jsonResult.Add("Lamp", "GOOD");
                }
            }
            return jsonResult;
        }

        private static double GetRound(double dblnum, int numberprecision)
        {
            //Modified by lucky on 2008-11-25 Math.Round(dblnum, numberprecision, MidpointRounding.AwayFromZero) is banker
            int tmpNum = dblnum > 0 ? 5 : -5;
            double dblreturn = Math.Truncate(dblnum * Math.Pow(10, numberprecision + 1)) + tmpNum;
            dblreturn = Math.Truncate(dblreturn / 10) / Math.Pow(10, numberprecision);
            return dblreturn;
        }


        public static int UserInsert(UserEntity user) 
        {
            return DataAccess.OpenConnection()
                .Execute("INSERT INTO dbo.T_User ( UserID, UserName, LeadID, ImgUrl ) VALUES  ( @UserID,@UserName,@LeadID,@ImgUrl)"
                , new { @UserID = user.UserID, @UserName = user.UserName, @LeadID = user.LeadID, @ImgUrl = user.ImgUrl });
        }

        /// <summary>
        /// 获取今日评价次数
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static int GetTodayCount(string userid)
        {

            return DataAccess.OpenConnection()
                .Query<int>("SELECT COUNT(1) FROM dbo.T_Appraise WHERE AppraiseDate = @date AND AppraiseType = 2 AND AppraiseUserID = @UserID", new { @date = DateTime.Now.ToString("yyyy-MM-dd"), @UserID = userid }).SingleOrDefault<int>();

        }

        /// <summary>
        /// 根据搜索条件获取用户集合
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetUserList(string param)
        {
            string url = WebServiceConfig.user_search_send_url;
            url += "?access_token=" + HttpContext.Current.Request.Cookies.Get("access_token").Value;
            url += "&keywords=" + param;
            url += "&pageindex=1";
            url += "&pagesize=20";
            url += "&format=" + WebServiceConfig.format;

            string resultUser = HttpHandle.RequestGet(url);
            
            return resultUser;

        }
    }
}
