using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Contacts.ContactorClass;



namespace Contacts
{
    class Program
    {
        static void Main(string[] args)
        {
            string DataSource = ConfigurationManager.AppSettings["DataSource"];
            Contactor contactor = new Contactor(DataSource);
            
            // AES Test
            //string PlainText = ConfigurationManager.AppSettings["PlainText"];
            //string Encrypted = ConfigurationManager.AppSettings["Encrypted"];
            //Console.WriteLine("PlainText : " + PlainText);
            //Console.WriteLine("Encrypted : " + AESEncoder.AESEncryptBase64(PlainText));
            //Console.WriteLine("Encrypted : " + Encrypted);
            //Console.WriteLine("Decrypted : " + AESEncoder.AESDecrptBase64(Encrypted));
            

            // 先讀檔案中人員聯絡資料
            Console.WriteLine(" ========== 歡迎使用資訊處通訊錄查詢系統 ==========");
            //Console.WriteLine("讀取人員資料中...");
            Console.WriteLine();

            List<People> Contacts = contactor.GetContact();

            // 開始查詢輸出
            while (true)
            {
                Console.WriteLine("========== 請輸入數字以選擇查詢模式： ==========\n 1.依人名關鍵字\n 2.依單位關鍵字表列\n " +
                    "3.依分機號碼查詢\n 4.特殊需求查詢\n 5.重新取得分機表\n 6.更新本地分機表\n 7.離開程式");
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine(" ===== 請輸入人名關鍵字 =====");
                        string name = Console.ReadLine();
                        var query1 = Contacts.Where(p => p.Name.ToLower().Contains(name) || p.Name.ToUpper().Contains(name)).OrderBy(p => p.Depart).ThenByDescending(p => p.Note);
                        DisplayPeopleInfo(query1);
                        break;
                    case "2":
                        Console.WriteLine(" ===== 請輸入數字使用相關模式 =====\n 1.輸入單位關鍵字\n 2.表列所有單位清單再從中選擇");
                        string mode = Console.ReadLine();
                        switch (mode)
                        {
                            case "1":
                                Console.WriteLine(" ===== 請輸入單位關鍵字 =====");
                                string depart = Console.ReadLine();
                                DisplayDeparts(Contacts, depart);
                                break;
                            case "2":
                                DisplayDeparts(Contacts);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "3":
                        Console.WriteLine(" ===== 請輸入分機號碼 =====");
                        string exten = Console.ReadLine();
                        var query2 = Contacts.Where(x => x.Extension.StartsWith(exten));
                        DisplayPeopleInfo(query2);
                        break;
                    case "4":
                        var query3 = Contacts.Where(x => !string.IsNullOrEmpty(x.SpecialSys));
                        DisplayPeopleInfo(query3);
                        break;
                    case "5":
                        Contacts.Clear();
                        Console.WriteLine(" ========== 正在重新取得分機表 ==========");
                        Contacts = contactor.GetContact();
                        break;
                    case "6":
                        Console.WriteLine(" ========== 正在更新本地分機表 ==========");
                        if (contactor.UpdateCsvFile())
                            Console.WriteLine(" ========== 更新完成 ==========");
                        else
                            Console.WriteLine(" ========== 本地分機表無法更新，請洽開發人員 ==========");
                        break;
                    case "7":
                        Console.WriteLine(" ========== 感謝使用，歡迎再次光臨 ==========\n請按任意鍵繼續...");
                        Console.ReadKey();
                        System.Environment.Exit(0);
                        break;
                    case "cls":
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("========== 無效的指令，請再輸入一次 =========\n");
                        break;
                }
            }
        }

        public static void DisplayDeparts(List<People> Contacts, string depart = "")
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
                var query = Contacts.Where(p => p.Depart.Contains(depart)).OrderBy(p => p.Depart).ThenByDescending(p => p.Note);
                Console.WriteLine();
                DisplayPeopleInfo(query);
            }
        }

        public static void DisplayPeopleInfo(IEnumerable<People> query)
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
                    if (!string.IsNullOrEmpty(p.Note))
                        stbr.Append(p.Note);
                    stbr.AppendLine();
                }
            }
            Console.WriteLine(stbr.ToString());
        }


    }
}
