using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
	class Parcel
	{
        public int Id { get; set; }
        public CustomerInParcel Sender { get; set; }
        public CustomerInParcel Target { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public DroneInParcel Drone { get; set; }
        public DateTime? RequestedTime { get; set; }
        public DateTime? ScheduleTime { get; set; }
        public DateTime? PickUpTime { get; set; }
        public DateTime? DeliveredTime { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
