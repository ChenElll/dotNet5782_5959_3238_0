using System.Collections.Generic;

namespace BO
{
    public class BaseStation
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
