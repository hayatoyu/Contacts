using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts.ContactorClass
{
    interface ContactGetter
    {
        List<E_Extension> GetContact();
        List<E_Status> GetStatus();
    }
}
