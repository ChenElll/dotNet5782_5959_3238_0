using System;

namespace Dal
{
    namespace DO
    {
        public struct Parcel
        {
            public int Id { get; set; }
            public int SenderId { get; set; } //sender
            public int TargetId { get; set; } //target
            public WeightCategories Weight { get; set; } //parcel's weight
            public Priorities Priority { get; set; } // priority
            public int DroneId { get; set; }
            public DateTime? RequestedTime { get; set; } // creating drone time
            public DateTime? ScheduledTime { get; set; } // scheduled drone time
            public DateTime? PickedUpTime { get; set; }  // pick up drone time
            public DateTime? DeliveredTime { get; set; } // delivery drone time
                                                        

            public override string ToString()
            {
                return this.ToStringProperty();
            }
        }
    }

}