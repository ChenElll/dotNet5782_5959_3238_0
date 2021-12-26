using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
	class CustomerInParcel
	{
        public int Id { get; set; }
        public string CustomerName { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
