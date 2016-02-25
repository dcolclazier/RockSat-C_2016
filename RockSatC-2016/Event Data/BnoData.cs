using RockSatC_2016.Abstract;

namespace RockSatC_2016.Event_Data {
    public class BNOData : IEventData
    {
        public bool loggable => true;
        public double gyro_x { get; set; }
        public double gyro_y { get; set; }
        public double gyro_z { get; set; }
        public double accel_x { get; set; }
        public double accel_y { get; set; }
        public double accel_z { get; set; }

    }

    public class BNOTempData : IEventData
    {
        public bool loggable => true;
        public double temp { get; set; }
    }
}