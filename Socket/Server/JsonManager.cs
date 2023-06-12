using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server
{
    class JsonManager
    {
        string json;
        JObject jobject;
        public JsonManager() { }

        public void LoadJson()
        {
            try
            {

                json = File.ReadAllText(@"C:\Users\윤현후\source\repos\Socket\Server\HostList.json");
                Console.WriteLine("열었습니다.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            jobject = JObject.Parse(json);
        }
        public List<string> GetIP()
        {
            List<string> IpList = new List<string>();

            foreach (string s in jobject["IP"])
            {
                IpList.Add(s);
                Console.WriteLine(s);
            }

            return IpList;
        }
    }
}