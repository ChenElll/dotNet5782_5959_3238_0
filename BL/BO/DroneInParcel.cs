namespace BO
{
    public class DroneInParcel
    {
        public int Id { get; set; }
        public double Battery { get; set; }
        public Location Location { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
