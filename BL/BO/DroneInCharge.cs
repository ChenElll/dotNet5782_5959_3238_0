namespace BO
{
    public class DroneInCharge
    {
        public int Id { get; set; }
        public double BatteryStatus { get; set; }
        public bool Deleted { set; get; } = false;

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
