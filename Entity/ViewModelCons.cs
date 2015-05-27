using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class ViewModelCons
    {
        /// <summary>
        /// 1 :成功，0：失败
        /// </summary>
        public string resultFlag { get; set; } 
        public Guid CurrentUserID { get; set; }
        public UserConsEntity CurrentUserConsEntity { get; set; }

        public ConstellationEntity CurrentConsteEntity { get; set; }

        /// <summary>
        /// 福星高照
        /// </summary>
        public Guid GoodLuckUserId { get; set; }

        public UserConsEntity GoodLuckUserConsEntity { get; set; }


        /// <summary>
        /// 约她
        /// </summary>
        public Guid MissUserId { get; set; }
        public UserConsEntity MissUserConsEntity { get; set; }

        /// <summary>
        /// 献出一点爱
        /// </summary>
        public Guid LoveUserId { get; set; }
        public UserConsEntity LoveUserConsEntity { get; set; }



    }
}
