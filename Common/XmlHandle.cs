using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Common
{
    public class XmlHandle
    {
        ///序列化
        public static string XMLSerialize<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                formatter.Serialize(ms, obj);
                return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        ///反序列化
        public static T XMLDeserialize<T>(string XML)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(XML);
                ms.Write(byteArray, 0, byteArray.Length);
                ms.Position = 0;
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                try
                {
                    return (T)formatter.Deserialize(ms);
                }
                catch { }
                return default(T);
            }
        }
        
    }
}
