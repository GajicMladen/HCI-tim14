using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim14HCI.Model
{
    public class Train
    {
        public int TrainID { get; set; }
        public string Name { get; set; }
        public float MaxSpeed { get; set; }
        public int Capacity { get; set; }

        public virtual List<TrainLine> TrainLines { get; set; }

    }
}
