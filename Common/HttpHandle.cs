using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Common
{
    public class HttpHandle
    {
        public delegate void RequestCallBack(string html);
        public static void RequestAsyncGet(string Url, RequestCallBack Rck)
        {
            WebRequest wrq = WebRequest.Create(Url);

            wrq.BeginGetResponse(new AsyncCallback(delegate(IAsyncResult ar)
            {
                WebRequest cwrq = (WebRequest)ar.AsyncState;

                WebResponse wrs = cwrq.EndGetResponse(ar);

                Stream steam = wrs.GetResponseStream();//从还回对象中获取数据流

                StreamReader reader = new StreamReader(steam, Encoding.UTF8);//读取数据Encoding.GetEncoding("gb2312")指编码是gb2312，不让中文会乱码的

                if (Rck != null)
                {
                    Rck(reader.ReadToEnd());
                }

                reader.Close();

                cwrq.Abort();

            }), wrq);
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="Url">Url地址</param>
        /// <returns></returns>
        public static string RequestGet(string Url)
        {
            string PageStr = string.Empty;//用于存放还回的html
            Uri url = new Uri(Url);//Uri类 提供统一资源标识符 (URI) 的对象表示形式和对 URI 各部分的轻松访问。就是处理url地址
            try
            {
                HttpWebRequest httprequest = (HttpWebRequest)WebRequest.Create(url);//根据url地址创建HTTpWebRequest对象
                HttpWebResponse response = (HttpWebResponse)httprequest.GetResponse();//使用HttpWebResponse获取请求的还回值
                Stream steam = response.GetResponseStream();//从还回对象中获取数据流
                StreamReader reader = new StreamReader(steam, Encoding.UTF8);//读取数据Encoding.GetEncoding("gb2312")指编码是gb2312，不让中文会乱码的
                PageStr = reader.ReadToEnd();
                reader.Close();
            }
            catch { }
            return PageStr;
        }
        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="Url">Url地址</param>
        /// <param name="Context">Post数据</param>
        /// <returns></returns>
        public static string RequestPost(string Url, string Context)
        {
            string PageStr = string.Empty;
            Uri url = new Uri(Url);
            byte[] reqbytes = Encoding.ASCII.GetBytes(Context);
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "post";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = reqbytes.Length;
                Stream stm = req.GetRequestStream();
                stm.Write(reqbytes, 0, reqbytes.Length);

                stm.Close();
                HttpWebResponse wr = (HttpWebResponse)req.GetResponse();
                Stream stream = wr.GetResponseStream();
                StreamReader srd = new StreamReader(stream, Encoding.GetEncoding("gb2312"));
                PageStr += srd.ReadToEnd();
                stream.Close();
                srd.Close();
            }
            catch { }
            return PageStr;
        }
    }
}
