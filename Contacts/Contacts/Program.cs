using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    class Program
    {
        static void Main(string[] args)
        {
            List<People> Contacts = new List<People>();
            string filepath = System.Environment.CurrentDirectory + @"\Contacts.csv";
            // 先讀檔案中人員聯絡資料
            Console.WriteLine(" ========== 歡迎使用資訊處通訊錄查詢系統 ==========");
            Console.WriteLine("讀取人員資料中...");
            Console.WriteLine();
            try
            {
                var CE = EncodingDetection(filepath);
                using (StreamReader sr = new StreamReader(filepath, CE))
                {
                    while (sr.Peek() >= 0)
                    {

                        string line = sr.ReadLine();
                        if (!line.Contains("<EOF>"))
                        {
                            line = line.TrimEnd().Replace("\t", ",");
                            string[] contact = line.Split(',');
                            Contacts.Add(
                                new People
                                {
                                    Name = contact[0],
                                    Extension = contact[1],
                                    Depart = contact[2],
                                    Note = contact.Length > 3 ? contact[3] : string.Empty
                                }
                                );
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

            // 開始查詢輸出
            while (true)
            {
                Console.WriteLine("========== 請輸入數字以選擇查詢模式： ==========\n 1.依人名關鍵字\n 2.依單位關鍵字表列\n 3.離開程式");
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine(" ===== 請輸入人名關鍵字 =====");
                        string name = Console.ReadLine();
                        var query1 = Contacts.Where(p => p.Name.Contains(name)).OrderBy(p => p.Depart).ThenByDescending(p => p.Note);
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
                        Console.WriteLine(" ========== 感謝使用，歡迎再次光臨 ==========\n請按任意鍵繼續...");
                        Console.ReadKey();
                        System.Environment.Exit(0);
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
                             group people by people.Depart).ToList();
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
            Console.WriteLine();
            StringBuilder stbr = new StringBuilder();
            foreach (People p in query)
            {
                stbr.Append(p.Name + "\t" + p.Extension + "\t" + p.Depart + "\t");
                if (!string.IsNullOrEmpty(p.Note))
                    stbr.Append(p.Note);
                stbr.AppendLine();
            }
            Console.WriteLine(stbr.ToString());
        }

        public static Encoding EncodingDetection(string filepath)
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
            catch
            {
                throw;
            }

            return CE;

        }
    }
}
