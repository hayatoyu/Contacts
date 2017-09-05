using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts.ContactorClass
{
    interface ContactGetter
    {
        List<People> GetContact();
        List<P_Status> GetStatus();
    }
}
