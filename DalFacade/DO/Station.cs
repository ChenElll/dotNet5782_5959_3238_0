
namespace Dal
{
    namespace DO
    {
        public struct Station
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Longtitude { get; set; }
            public double Lattitude { get; set; }
            public int FreeChargeSlots { get; set; }

            /// <summary>
            /// The status of the station in the list, if it is deleted or not
            /// </summary>
         //   public bool Deleted { set; get; }

            public override string ToString()
            {
                return this.ToStringProperty();
            }
        }
    }
}