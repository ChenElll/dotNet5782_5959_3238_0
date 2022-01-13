namespace BO
{
    public class BaseStationToList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FreeChargeSlots { get; set; }
        public int OccupiedChargeSlots { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
