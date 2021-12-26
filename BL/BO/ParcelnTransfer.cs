using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
	class ParcelnTransfer
	{
        public int Id { get; set; }
        public bool ParcelStatus { get; set; }  // WaitForCollection=1, OnTheWayToDestination=0
        public Priorities Priority { get; set; }
        public WeightCategories ParcelWeight { get; set; }
        public CustomerInParcel Sender { get; set; }

        public CustomerInParcel Target { get; set; }
        public Location Collection { get; set; }
        public Location DeliveryDestination { get; set; }
        public int TransportDistance { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
