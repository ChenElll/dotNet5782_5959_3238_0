using DO;

namespace BO
{
    public class ParcelToList
    {
        public int Id { get; set; }
        public string NameSender { get; set; }
        public string NameTarget { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public ParcelStatus ParcelStatus { get; set; }


        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
