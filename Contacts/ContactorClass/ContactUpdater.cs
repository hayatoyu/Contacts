using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using NPOI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Data.SqlClient;

namespace Contacts.ContactorClass
{
    // 讀取 Excel 通訊錄，並更新至資料庫
    class ContactUpdater
    {
        List<E_Extension> Contact;
        List<E_Extension> DB;
        private string connStr,userID,pswd;
        private const string ContactPath = "ITRDContact.xls";
        
        public ContactUpdater()
        {
            Contact = new List<E_Extension>();
            DB = new List<E_Extension>();
            connStr = ConfigurationManager.AppSettings["ConnStrings"];
            userID = ConfigurationManager.AppSettings["UserID"];
            pswd = ConfigurationManager.AppSettings["Password"];
        }

        public void ReadFromExcel()
        {
            using (FileStream fs = new FileStream(ContactPath,FileMode.Open,FileAccess.Read))
            {
                IWorkbook wb = new HSSFWorkbook(fs);
                ISheet ws = wb.GetSheetAt(0);
                IRow row = null;
                string depart = string.Empty, temp;
                for(int i = 1;i < 5;i++)
                {
                    for(int j = 3;j < 44;j++)
                    {
                        row = ws.GetRow(j);
                        temp = row.GetCell(i).StringCellValue;
                        if(!string.IsNullOrEmpty(temp))
                        {
                            if (temp.Substring(1, 1).ToUpper().Equals("F"))
                            {
                                depart = temp.Substring(2, temp.IndexOf("(") - 2) + "科";
                                continue;
                            }
                            Contact.Add(new E_Extension
                            {
                                Name = temp.Substring(0, temp.Length - 4),
                                Extension = temp.Substring(temp.Length - 4, 4),
                                Depart = depart
                            });
                        }
                        
                    }

                }

                // 處長與工讀生
                // 處長後面還帶日期，不好弄，反正不會常更新，要是處長有換再手動更新
                // 工讀生的直接抓好了
                row = ws.GetRow(43);
                temp = row.GetCell(1).StringCellValue;
                Contact.Add(new E_Extension
                {
                    Name = temp.Substring(0, temp.Length - 4),
                    Extension = temp.Substring(temp.Length - 4, 4)
                });

                wb.Close();
            }
        }

        public void UpdateDB()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                connStr = connStr.Replace("usrid", AESEncoder.AESDecrptBase64(userID))
                                 .Replace("pswd", AESEncoder.AESDecrptBase64(pswd));
                conn.ConnectionString = connStr;
                //DB = conn.Query<E_Extension>("select * from T_Extension").ToList();
                DB = conn.Query<E_Extension>("usp_GetExtension_D", commandType: System.Data.CommandType.StoredProcedure).ToList();

                foreach(E_Extension ext in Contact)
                {
                    var exist = DB.Where(e => e.Name.Equals(ext.Name)).FirstOrDefault();
                    if(exist.ID != 0)
                    {
                        ext.ID = exist.ID;
                        ext.SpecialSys = exist.SpecialSys;
                        ext.Notes = exist.Notes;
                    }
                }

                conn.Execute("usp_UpdateExt", Contact.ToArray(), commandType: System.Data.CommandType.StoredProcedure);
            }

        }

    }
}
