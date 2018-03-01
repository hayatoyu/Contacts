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
        public List<E_Extension> GetContact()
        {
            Console.WriteLine("========== 正在向遠端請求資料 ==========");
            List<E_Extension> Contacts = new List<E_Extension>();
            string url = "http://10.10.4.57:9999/HyperledgerFabric/RestfulTestServiceImpl.svc/Contact";

            try
            {
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
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                System.Environment.Exit(1);
            }
            return Contacts;
        }

        public List<E_Status> GetStatus()
        {
            List<E_Status> P_Statuses = new List<E_Status>();
            string url = "http://10.10.4.57:9999/HyperledgerFabric/RestfulTestServiceImpl.svc/ContactStatus";

            try
            {
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
                    var obj = JsonConvert.DeserializeObject<GetStatuses>(sr.ReadToEnd());
                    P_Statuses = obj.Statuses.ToList();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                System.Environment.Exit(1);
            }
            return P_Statuses;
        }
    }
    
    class GetRestResult
    {
        public GetContactResult GetContactResult { get; set; }
    }

    class GetContactResult
    {
        public string errMsg { get; set; }
        public E_Extension[] list { get; set; }
    }

    class GetStatuses
    {
        public E_Status[] Statuses { get; set; }
    }
}
