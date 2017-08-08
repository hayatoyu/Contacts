using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Contacts.ContactorClass
{
    class GetFromCSV : ContactGetter
    {
        public List<People> GetContact()
        {
            List<People> Contacts = new List<People>();
            Console.WriteLine("========== 正在讀取本地 csv 文件 ==========");
            string filepath = System.Environment.CurrentDirectory + @"\Contacts.csv";
            try
            {
                using (StreamReader sr = new StreamReader(filepath, EncodingDetection(filepath)))
                {
                    while (sr.Peek() >= 0)
                    {
                        string line = sr.ReadLine();
                        if (!line.Contains("<EOF>"))
                        {
                            line = line.TrimEnd().Replace("\t", ",");
                            string[] contact = line.Split(',');
                            People p = new People
                            {
                                Name = contact[0],
                                Extension = contact[1],
                                Depart = contact[2],
                                SpecialSys = contact.Length > 3 ? contact[3] : string.Empty,
                                Note = contact.Length > 4 ? contact[4] : string.Empty
                            };

                            Contacts.Add(p);
                        }
                        else
                            break;
                    }
                }
                Console.WriteLine("========== 讀取完成，請參考說明使用 ==========\n");
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(" ===== 發生錯誤，程式即將關閉 =====\n請按任意鍵繼續...");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            return Contacts;
        }

        private Encoding EncodingDetection(string filepath)
        {
            Encoding CE = null;
            try
            {
                using (StreamReader sr = new StreamReader(filepath, Encoding.Default))
                {
                    sr.Read();
                    CE = sr.CurrentEncoding;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return CE;
        }
    }
}
