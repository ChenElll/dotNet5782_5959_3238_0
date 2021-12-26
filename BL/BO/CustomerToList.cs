namespace BO
{
    public class CustomerToList
	{
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public int NumberSentAndProvidedParcels { get; set; }
        public int NumberSentAnd_Not_ProvidedParcels { get; set; }
        public int NumberParcelsReceived { get; set; }
        public int NumberParcelsOnWay { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
