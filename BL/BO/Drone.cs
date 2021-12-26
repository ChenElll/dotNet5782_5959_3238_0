using DO;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BO
{
    public class Drone
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public WeightCategories MaxWeight { get; set; }
        public double Battery { get; set; }
        public DroneStatuses Status { get; set; }
        public ParcelInTransfer ParcelInTransfer { get; set; }
        public Location Location { get; set; }


        public override string ToString()
        {
            return this.ToStringProperty();
        }

    }
}
