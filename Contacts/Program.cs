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
            //Console.WriteLine(AESEncoder.AESEncryptBase64("n21Dq435t9S1"));
            

            // 先讀檔案中人員聯絡資料
            Console.WriteLine(" ========== 歡迎使用資訊處通訊錄查詢系統 ==========");           
            Console.WriteLine();

            List<E_Extension> Contacts = contactor.GetContact();
            

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
                        var query1 = Contacts.Where(p => p.Name.ToLower().Contains(name) || p.Name.ToUpper().Contains(name)).OrderBy(p => p.Depart).ThenByDescending(p => p.Notes);
                        contactor.DisplayPeopleInfo(query1);
                        break;
                    case "2":
                        Console.WriteLine(" ===== 請輸入數字使用相關模式 =====\n 1.輸入單位關鍵字\n 2.表列所有單位清單再從中選擇");
                        string mode = Console.ReadLine();
                        switch (mode)
                        {
                            case "1":
                                Console.WriteLine(" ===== 請輸入單位關鍵字 =====");
                                string depart = Console.ReadLine();
                                contactor.DisplayDeparts(Contacts, depart);
                                break;
                            case "2":
                                contactor.DisplayDeparts(Contacts);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "3":
                        Console.WriteLine(" ===== 請輸入分機號碼 =====");
                        string exten = Console.ReadLine();
                        var query2 = Contacts.Where(x => x.Extension.StartsWith(exten));
                        contactor.DisplayPeopleInfo(query2);
                        break;
                    case "4":
                        var query3 = Contacts.Where(x => !string.IsNullOrEmpty(x.SpecialSys));
                        contactor.DisplayPeopleInfo(query3);
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
        
    }
}
