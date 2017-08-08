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
                    string line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", p.Name, p.Extension, p.Depart, p.SpecialSys, p.Note);
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
    }
}
