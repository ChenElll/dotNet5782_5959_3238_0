using DO;

namespace BO
{
    public class DroneToList
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public WeightCategories MaxWeight { get; set; }
        public double Battery { get; set; }
        public DroneStatuses Status { get; set; }
        public Location Location { get; set; }
        public int ParcelNumberInTransfer { get; set; }


        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
