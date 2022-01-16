using System;
using DO;

namespace BO
{
    public class Location
    {
        public double Longtitude { get; set; }
        public double Lattitude { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
