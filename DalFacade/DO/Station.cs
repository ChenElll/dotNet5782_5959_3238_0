

namespace DO
{
    public struct Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Longtitude { get; set; }
        public double Lattitude { get; set; }
        public int FreeChargeSlots { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
