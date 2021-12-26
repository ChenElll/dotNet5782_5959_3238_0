namespace BO
{
    public class CustomerInParcel
	{
        public int Id { get; set; }
        public string CustomerName { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
