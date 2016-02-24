using RockSatC_2016.Abstract;

namespace RockSatC_2016.Event_Data {
    public class BnoData : IEventData {
        public double Gyro_X { get; set; }
        public double Gyro_Y { get; set; }
        public double Gyro_Z { get; set; }

        //public double Accel_X { get; set; }
        //public double Accel_Y { get; set; }
        //public double Accel_Z { get; set; }
        public bool loggable => true;

    }
}