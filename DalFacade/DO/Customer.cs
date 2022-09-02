
namespace DO
{
    /// <summary>
    /// Custumer class impl.
    /// </summary>
    public struct Customer
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public double Longtitude { get; set; }
        public double Lattitude { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
