using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts.ContactorClass
{
    class Contactor
    {
        ContactGetter getter;

        public Contactor(string DataSource)
        {
            if (DataSource == "1")
                getter = new GetFromCSV();
            else if (DataSource == "2")
                getter = new GetFromDB();
            else if (DataSource == "3")
                getter = new GetFromRESTful();
            
        }

        public List<People> GetContact()
        {
            Statuses = getter.GetStatus();
            return getter.GetContact();
        }
        
        public bool UpdateCsvFile()
        {
            string DataSource = ConfigurationManager.AppSettings["DataSource"];
            if (!DataSource.Equals("1"))
            {
                string filepath = System.Environment.CurrentDirectory + @"\Contacts.csv";

                // 先刪掉原始檔案
                try
                {
                    File.Delete(filepath);
                }
                catch (DirectoryNotFoundException e)
                {
                    // 找不到這個檔案，可以直接存新檔就好
                }
                catch (IOException e)
                {
                    // 檔案使用中，中止此方法
                    return false;
                }
                catch (UnauthorizedAccessException e)
                {
                    // 檔案無存取權限，中止此方法
                    return false;
                }

                // 存新的檔案
                StringBuilder stbr = new StringBuilder();
                var Contacts = getter.GetContact();
                foreach (var p in Contacts)
                {
                    string line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", p.Name, p.Extension, p.Depart, p.SpecialSys, p.Notes);
                    stbr.AppendLine(line);
                }
                stbr.AppendLine("<EOF>");
                try
                {
                    using (var sw = new StreamWriter(filepath, false, Encoding.UTF8))
                    {
                        sw.WriteLine(stbr.ToString());
                        sw.Flush();
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
                return true;

            }
            else
            {
                Console.WriteLine(" ========== 目前資料來源為csv檔案，無法刪除。請在資料來源為資料庫時執行此功能 ==========");
                return false;
            }

        }

        private List<P_Status> Statuses = new List<P_Status>();

        public void DisplayDeparts(List<People> Contacts, string depart = "")
        {
            StringBuilder stbr = new StringBuilder();
            // 全表列
            if (string.IsNullOrEmpty(depart))
            {
                int departNo;
                var query = (from people in Contacts
                             group people by people.Depart
                             ).OrderByDescending(p => p.Key).ToList();
                Console.WriteLine();
                for (int i = 0; i < query.Count; i += 2)
                {
                    stbr.Append((i + 1).ToString() + "." + query[i].Key + "\t");
                    if (i + 1 < query.Count)
                        stbr.Append((i + 2).ToString() + "." + query[i + 1].Key + "\t");
                    stbr.AppendLine();
                }
                Console.WriteLine(stbr.ToString() + "\n");
                Console.WriteLine(" ===== 請輸入數字以選擇單位 =====");
                depart = Console.ReadLine();
                if (int.TryParse(depart, out departNo))
                {
                    if (departNo > 0 && departNo < query.Count + 1)
                    {
                        depart = query[departNo - 1].Key;
                        DisplayDeparts(Contacts, depart);
                        Console.WriteLine();
                    }
                    return;
                }
                Console.WriteLine(" ===== 無效的單位代號 =====\n\n");

            }
            // 依關鍵字表列
            else
            {
                var query = Contacts.Where(p => p.Depart.Contains(depart)).OrderBy(p => p.Depart).ThenByDescending(p => p.Notes);
                Console.WriteLine();
                DisplayPeopleInfo(query);
            }
        }

        public void DisplayPeopleInfo(IEnumerable<People> query)
        {
            Console.Clear();
            Console.WriteLine();
            StringBuilder stbr = new StringBuilder();
            if (query.Count() == 0)
            {
                stbr.AppendLine("=======查無資料！=======");
            }
            else
            {
                foreach (People p in query)
                {
                    stbr.Append(p.Name + "\t" + p.Extension + "\t" + p.Depart + "\t");
                    if (!string.IsNullOrEmpty(p.SpecialSys))
                        stbr.Append(p.SpecialSys + "\t");
                    if (!string.IsNullOrEmpty(p.Notes))
                        stbr.Append(p.Notes + "\t");
                    
                    // 查此人狀態
                    if(Statuses.Exists(s => s.EmployeeID == p.ID))
                    {
                        string status = Statuses.Where(s => s.EmployeeID == p.ID
                                                 && s.StartTime <= DateTime.Now
                                                 && s.EndTime >= DateTime.Now).FirstOrDefault().Status;
                        if (!string.IsNullOrEmpty(status))
                            stbr.Append(status + "\t");
                    }

                    stbr.AppendLine();
                }
            }
            Console.WriteLine(stbr.ToString());
        }
    }
}
