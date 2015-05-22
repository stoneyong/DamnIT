using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class UserEntity
    {
        public UserEntity() 
        {
            TodayCount = 0;
        }
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string ImgUrl { get; set; }
        public Guid LeadID { get; set; }

        public int TodayCount { get; set; }
    }

}
