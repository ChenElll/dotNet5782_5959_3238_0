using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
	class BaseStation
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }
        public int FreeChargeSlots { get; set; }
        public List<DroneInCharge> DronesInCharge { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
