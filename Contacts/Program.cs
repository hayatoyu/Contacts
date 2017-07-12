﻿using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace Contacts
{
    class Program
    {
        static void Main(string[] args)
        {
            List<People> Contacts = new List<People>();

            // AES Test
            //string PlainText = ConfigurationManager.AppSettings["PlainText"];
            //string Encrypted = ConfigurationManager.AppSettings["Encrypted"];
            //Console.WriteLine("PlainText : " + PlainText);
            //Console.WriteLine("Encrypted : " + AESEncoder.AESEncryptBase64(PlainText));
            //Console.WriteLine("Encrypted : " + Encrypted);
            //Console.WriteLine("Decrypted : " + AESEncoder.AESDecrptBase64(Encrypted));
            

            // 先讀檔案中人員聯絡資料
            Console.WriteLine(" ========== 歡迎使用資訊處通訊錄查詢系統 ==========");
            Console.WriteLine("讀取人員資料中...");
            Console.WriteLine();

            Contacts = GetContact();

            // 開始查詢輸出
            while (true)
            {
                Console.WriteLine("========== 請輸入數字以選擇查詢模式： ==========\n 1.依人名關鍵字\n 2.依單位關鍵字表列\n " +
                    "3.依分機號碼查詢\n 4.特殊需求查詢\n 5.重新取得分機表\n 6.離開程式");
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
                        Contacts = GetContact();
                        break;
                    case "6":
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

        public static List<People> GetContact()
        {
            string DataSource = ConfigurationManager.AppSettings["DataSource"];

            List<People> Contacts = new List<People>();

            // local csv file
            if (DataSource.Equals("1"))
            {
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
                                    Note = contact.Length > 3 ? contact[3] : string.Empty
                                };
                                p.NoteToSpecialSys();
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
            }

            // DB
            else if (DataSource.Equals("2"))
            {
                try
                {
                    Console.WriteLine("========== 正在讀取資料庫 ==========");
                    string ConnString = ConfigurationManager.AppSettings["ConnStrings"];
                    string UserID = ConfigurationManager.AppSettings["UserID"];
                    string Password = ConfigurationManager.AppSettings["Password"];
                    using (SqlConnection conn = new SqlConnection())
                    {
                        ConnString = ConnString.Replace("usrid", AESEncoder.AESDecrptBase64(UserID)).Replace("pswd", AESEncoder.AESDecrptBase64(Password));
                        conn.ConnectionString = ConnString;
                        conn.Open();
                        string query = "select * from T_Extension";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Contacts.Add(new People
                            {
                                Name = row["Name"].ToString(),
                                Extension = row["Extension"].ToString(),
                                Depart = row["Depart"].ToString(),
                                SpecialSys = row["SpecialSys"] == null ? string.Empty : row["SpecialSys"].ToString(),
                                Note = row["Notes"] == null ? string.Empty : row["Notes"].ToString()
                            });
                        }
                    }
                    Console.WriteLine("========== 讀取完成，請參考說明使用 ==========\n");
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(" ===== 發生錯誤，程式即將關閉 =====\n請按任意鍵繼續...");
                    Console.ReadKey();
                    System.Environment.Exit(1);
                }
            }
            return Contacts;
        }
    }
}
