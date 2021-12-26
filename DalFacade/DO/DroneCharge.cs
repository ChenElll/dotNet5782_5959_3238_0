using System;

namespace DO
{
    public struct DroneCharge
    {
        public int DroneId { get; set; }
        public int StationId { get; set; }
        public DateTime? EntranceTime { get; set; }
        public DateTime? LeavingTime { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
