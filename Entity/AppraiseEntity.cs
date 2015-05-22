using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class AppraiseEntity
    {
        public Guid AppraiseID { get; set; }
        /// <summary>
        /// 1.给自己评价  2给他人评价
        /// </summary>
        public int AppraiseType { get; set; }
        /// <summary>
        /// 1.工作量  2.工作效率  3合作性
        /// </summary>
        public int AppraiseClass { get; set; }
        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime AppraiseDate { get; set; }
        /// <summary>
        /// 得分
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// 评论用户
        /// </summary>
        public Guid AppraiseUserID { get; set; }
        /// <summary>
        /// 被评论用户
        /// </summary>
        public Guid ByAppraiseUserID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CrateDate { get; set; }
    }
}
