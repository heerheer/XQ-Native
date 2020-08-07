using System.Collections.Generic;
using System.IO;

namespace XQNative_Console
{
    public static class JsonDoctor
    {
        public static string DealJson(string path)
        {
            var jsono = File.ReadAllLines(path);
            List<string> newjson = new List<string>();
            foreach (var item in jsono)
            {
                if (item.StartsWith("//"))
                {
                    //不加入
                }
                else
                {
                    if (item.Contains("//"))
                    {
                        //删去加入
                        newjson.Add(
                        item.Substring(0, item.IndexOf("//"))
                        );
                    }
                    else
                    {
                        newjson.Add(item);
                    }
                }
            }
            return string.Join("\n", newjson);
        }
    }
}