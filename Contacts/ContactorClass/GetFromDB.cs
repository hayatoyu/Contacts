using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Contacts.ContactorClass
{
    class GetFromDB : ContactGetter
    {
        public List<E_Extension> GetContact()
        {
            string DataSource = ConfigurationManager.AppSettings["DataSource"];
            
            List<E_Extension> Contacts = new List<E_Extension>();
            
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

                    //Contacts = conn.Query<E_Extension>("select * from T_Extension").ToList();
                    Contacts = conn.Query<E_Extension>("usp_GetExtension_D", commandType: CommandType.StoredProcedure).ToList();

                    
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

        public List<E_Status> GetStatus()
        {
            string ConnString = ConfigurationManager.AppSettings["ConnStrings"];
            string UserID = ConfigurationManager.AppSettings["UserID"];
            string Password = ConfigurationManager.AppSettings["Password"];
            List<E_Status> P_Statuses = new List<E_Status>();

            using (SqlConnection conn = new SqlConnection())
            {
                ConnString = ConnString.Replace("usrid", AESEncoder.AESDecrptBase64(UserID)).Replace("pswd", AESEncoder.AESDecrptBase64(Password));
                conn.ConnectionString = ConnString;

                P_Statuses = conn.Query<E_Status>("select * from V_TodayStatus").ToList();
                
            }
            return P_Statuses;
        }
    }
}
