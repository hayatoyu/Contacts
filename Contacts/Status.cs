using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Contacts
{
    class P_Status
    {
        public int ID { get; private set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public int EmployeeID { get; set; }
        public string Name { get; set; }

        public P_Status(DataRow row)
        {
            int tempID, EmployeeID;
            DateTime StartTime, EndTime;            
            if (int.TryParse(row["ID"].ToString(), out tempID))
                this.ID = tempID;
            if (int.TryParse(row["EmployeeID"].ToString(), out EmployeeID))
                this.EmployeeID = EmployeeID;
            if (DateTime.TryParse(row["StartTime"].ToString(), out StartTime))
                this.StartTime = StartTime;
            if (DateTime.TryParse(row["EndTime"].ToString(), out EndTime))
                this.EndTime = EndTime;
            this.Status = row["Status"].ToString();
            this.Name = row["Name"].ToString();
        }
        public P_Status()
        {

        }
    }
}
