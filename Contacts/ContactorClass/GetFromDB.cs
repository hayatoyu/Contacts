﻿using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts.ContactorClass
{
    class GetFromDB : ContactGetter
    {
        public List<People> GetContact()
        {
            string DataSource = ConfigurationManager.AppSettings["DataSource"];
            List<People> Contacts = new List<People>();
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(" ===== 發生錯誤，程式即將關閉 =====\n請按任意鍵繼續...");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            return Contacts;
        }
    }
}
