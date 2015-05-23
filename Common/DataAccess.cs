using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Common
{
    /**/
    /// <summary>
    /// DataAccess 的摘要说明
    /// </summary>
    public class DataAccess
    {
        
        /**/
        /// <summary>
        /// 打开数据库
        /// </summary>
        public static SqlConnection OpenConnection()
        {
            return new SqlConnection("server=115.28.108.146,8000;database=DamnIT;uid=B2C;pwd=88403633%@!mmFF");
            //return new SqlConnection("server=.;database=DamnIT;uid=sa;pwd=shuai");
        }

        public static SqlConnection ConsConnection()
        {
            return new SqlConnection("server=.;database=CaiPiao;uid=sa;pwd=''");
        }
       
    }
}
