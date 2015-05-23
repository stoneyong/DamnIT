using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Common;
using Entity;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Service
{
    public class ConsteService
    {
        static Dictionary<string, ConstellationEntity> DicCache = new Dictionary<string, ConstellationEntity>();
        public static ConstellationEntity GetConsByApi(string consName)
        {
            string url = HaoServiceConfig.HaoServiceUrl;
            url += "?key="+HaoServiceConfig.key;
            url += "&consName=" + consName;
            url += "&type=today";

            ConstellationEntity cons = null;
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
                    DicCache[consName] = cons;
                    ConsInsert(cons);
                }
            }
            catch { }
            if (cons == null)
            {
                cons = GetConsByNameFromDb(consName);
            }
            
            return cons;
            

        }

        public static ConstellationEntity GetCons(string consName)
        {
            ConstellationEntity resultCons = null;
            if(DicCache.ContainsKey(consName))
            {
                resultCons = DicCache[consName];
                if (resultCons.QueryDatetime != DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    resultCons = GetConsByApi(consName);
                }
            }
            return resultCons;
        }

        /// <summary>
        /// 获取星座
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static string GetConsteByDate(DateTime birthday)
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
            string sql = "INSERT INTO [dbo].[Conste]([Name] ,[QueryDateTime],[Date],[All],[Color] ,[Health] ,[Love],[Money] ,[QFriend],[Summary] ,[Work],[CreateTime])VALUES(@Name ,@QueryDateTime,@Date,@All ,@Color ,@Health,@Love ,@Money,@QFriend ,@Summary,@Work,@CreateTime)";
            return DataAccess.ConsConnection()
                .Execute(sql, new { @Name=cons.Name,@QueryDateTime=cons.QueryDatetime,@Date=cons.QueryDate,@All=cons.All,@Color=cons.Color,@Health=cons.Health,@Love=cons.Love,@Money=cons.Money,@QFriend=cons.QFriend,@Summary=cons.Summary,@Work=cons.Work, @CreateTime=DateTime.Now});
        }

        public static ConstellationEntity GetConsByNameFromDb(string name)
        {
            string sql = "select top 1 * from [dbo].[Conste] where Name= @Name order by desc createTime";
            var result = DataAccess.ConsConnection()
                .Query<ConstellationEntity>(sql, new { @Name = name }).FirstOrDefault<ConstellationEntity>();
            return result;
        }

        //public static ConstellationEntity GetConsFromApi(DateTime birthDay)
        //{
        //    var consName = GetConsteByDate(birthDay);

        //}
    }
}
