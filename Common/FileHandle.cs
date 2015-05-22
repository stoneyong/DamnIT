using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    public class FileHandle
    {
        private static string FilePath = string.Empty;
        static FileHandle()
        {
            FilePath =  Environment.CurrentDirectory.Replace("bin\\Debug", "");
        }
        
        public static void SaveXml(string filename,string xml)
        {
            File.WriteAllText(string.Format("{0}Xml\\{1}.xml", FilePath, filename), xml);
        }

        public static string LoadXml(string filename)
        {
            return File.ReadAllText(string.Format("{0}Xml\\{1}.xml", FilePath, filename));
        }
    }
}
