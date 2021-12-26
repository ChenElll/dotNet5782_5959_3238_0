using System.Collections.Generic;

namespace BO
{
    public class Customer
	{
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public Location Location { get; set; }
        public List<ParcelAtCustomer> FromCustomer { get; set; }
        public List<ParcelAtCustomer> ToCustomer { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
