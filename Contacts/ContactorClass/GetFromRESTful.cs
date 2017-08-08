using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Contacts.ContactorClass
{
    class GetFromRESTful : ContactGetter
    {
        public List<People> GetContact()
        {
            Console.WriteLine("========== 正在向遠端請求資料 ==========");
            List<People> Contacts = new List<People>();
            string url = "http://10.10.5.64:9999/HyperledgerFabric/RestfulTestServiceImpl.svc/Contact";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (StreamWriter sw = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                sw.Write(string.Empty);
                sw.Flush();
                sw.Close();
            }

            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                var obj = JsonConvert.DeserializeObject<GetRestResult>(sr.ReadToEnd());
                Contacts = obj.GetContactResult.list.ToList();                
            }
            return Contacts;
        }

        
    }
    
    class GetRestResult
    {
        public GetContactResult GetContactResult { get; set; }
    }

    class GetContactResult
    {
        public string errMsg { get; set; }
        public People[] list { get; set; }
    }
}
