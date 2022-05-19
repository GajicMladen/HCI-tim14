using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim14HCI.Model
{
    public class Ticket
    {
        public int TicketID { get; set; }

        public bool ForReservation { get; set; }

        public int DepartureID { get; set; }
        public Departure Departure { get; set; }


        public int UserID { get; set; }
        public User User { get; set; }


    }
}
