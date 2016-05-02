using RockSatC_2016.Abstract;

namespace RockSatC_2016.Event_Data {
    
    public class BNOData : IEventData
    {
        public bool loggable => true;
        public float accel_x { get; set; }
        public float accel_y { get; set; }
        public float accel_z { get; set; }
        public float gyro_x { get; set; }
        public float gyro_y { get; set; }
        public float gyro_z { get; set; }
        public float temp { get; set; }

    }
}