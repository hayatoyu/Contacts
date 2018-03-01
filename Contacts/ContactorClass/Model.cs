using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts.ContactorClass
{
    public class E_Extension
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Depart { get; set; }
        public string SpecialSys { get; set; }
        public string Notes { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }

    public class E_Status
    {
        public int ID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public int EmployeeID { get; set; }
    }
}
