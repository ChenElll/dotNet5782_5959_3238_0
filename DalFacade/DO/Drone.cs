﻿
namespace DO
{
    /// <summary>
    /// Drone class impl.
    /// </summary>
    public struct Drone
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public WeightCategories MaxWeight { get; set; }


        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
