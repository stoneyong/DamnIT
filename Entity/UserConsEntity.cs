using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
   public class UserConsEntity
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string ImgUrl { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public DateTime BirthDay { get; set; }
        public string ConsName { get; set; }
    }
}
