using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim14HCI.Model
{
    public enum UserRole { 
    
        CLIENT,
        MANAGER
    }
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public UserRole UserRole { get; set; }

        public virtual List<Ticket> Tickets { get; set; }
    }
}
