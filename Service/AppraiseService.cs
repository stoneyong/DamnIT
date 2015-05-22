using Dapper;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Service
{
    public class AppraiseService
    {
        /// <summary>
        /// 获取当天某用户得到N分的数量
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="score"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<AppraiseEntity> GetAppraise(string userID, decimal score, int type)
        {
            return DataAccess.OpenConnection()
                .Query<AppraiseEntity>("SELECT * FROM dbo.T_Appraise WHERE AppraiseDate = @date AND AppraiseType = @Type AND Score = @Score AND ByAppraiseUserID = @UserID", new { @date = DateTime.Now.ToString("yyyy-MM-dd"), @Type = type, @Score = score, @UserID = userID }).ToList<AppraiseEntity>();
        }
        /// <summary>
        /// 给别人评价货给自己评价 joe 2014-08-31
        /// </summary>
        /// <param name="ByAppraiseUserID">被评价的人</param>
        /// <param name="AppraiseType">评价形式，1 给自己评价  2给他人评价</param>
        /// <param name="AppraiseClass">评价类型1.工作量  2.工作效率  3合作性</param>
        /// <param name="Score">评价得分</param>
        /// <returns></returns>
        public static int AppraiseOthers(string ByAppraiseUserID, int AppraiseType, int AppraiseClass, decimal Score)
        {
            string userID = "";
            if (AppraiseType == 1)
            {
                //给自己评价，规则：10分，9分，8分只能用一次。其他分值次数不限
                userID = ByAppraiseUserID;
                if (Score >= 8 && Score <= 10)
                {
                    if (GetAppraise(ByAppraiseUserID, Score, 1).Count == 1)
                    {
                        return -1;
                    }
                }
            }
            else
            {
                userID = UserService.GetUser().UserID.ToString();
                if (Score == 8 || Score == 9 || Score == 10)
                {
                    //只能打分一次
                    if (GetAppraise(ByAppraiseUserID, Score, 2).Count == 1)
                    {
                        return -1;
                    }
                }
                if (Score == 7)
                {
                    if (GetAppraise(ByAppraiseUserID, Score, 2).Count == 2)
                    {
                        return -2;
                    }
                }
                if (Score == 6)
                {
                    if (GetAppraise(ByAppraiseUserID, Score, 2).Count == 3)
                    {
                        return -3;
                    }
                }
            }
            //插入评价表信息
            return DataAccess.OpenConnection()
                .Execute("INSERT INTO dbo.T_Appraise(AppraiseType,AppraiseClass,AppraiseDate,Score,AppraiseUserID,ByAppraiseUserID) values(@Type,@Class,@Date,@Score,@UserID,@ByUserID)", new { @Type = AppraiseType, @Class = AppraiseClass, @Date = DateTime.Now.ToString("yyyy-MM-dd"), @Score = Score, @UserID = userID, @ByUserID = ByAppraiseUserID });

        }
        /// <summary>
        /// 获取某用户一段时间内被评价信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public static List<AppraiseEntity> GetAppraise(Guid userID, DateTime beginDate, DateTime endDate)
        {
            return DataAccess.OpenConnection()
                .Query<AppraiseEntity>("SELECT * FROM dbo.T_Appraise WHERE ByAppraiseUserID = @ByUserID AND AppraiseDate >= @BeginDate AND AppraiseDate <= @EndDate  ORDER BY AppraiseDate", new { @ByUserID = userID, @BeginDate = beginDate, @EndDate = endDate }).ToList<AppraiseEntity>();

        }
    }
}
