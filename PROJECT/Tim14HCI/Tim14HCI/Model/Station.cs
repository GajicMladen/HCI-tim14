using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim14HCI.Model
{
    public class Station
    {
        public int StationID { get; set; }
        public string Name { get; set; }

        //for Linked Stations
        public virtual ICollection<LinkedStation> stations1 { get; set; }
        public virtual ICollection<LinkedStation> stations2 { get; set; }
    }
}
