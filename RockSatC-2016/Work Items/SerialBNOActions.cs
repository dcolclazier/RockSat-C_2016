using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Sensors;
using RockSatC_2016.Utility;
using SecretLabs.NETMF.Hardware.Netduino;

namespace RockSatC_2016.Work_Items {
    public class SerialBNOActions {

        private readonly SerialBNO _bnoSensor;

        private readonly BNOData _bnoData;

        private readonly ThreadPool.WorkItem _workItem;

        public SerialBNOActions() {
            _bnoSensor = new SerialBNO(SerialPorts.COM4,5000,5000,SerialBNO.Bno055OpMode.Operation_Mode_Ndof);
            _bnoData = new BNOData();

            _workItem = new ThreadPool.WorkItem(GyroUpdater, EventType.BNOUpdate, _bnoData, true);

            _bnoSensor.begin();
        }

        private void GyroUpdater() {
            
            var gyro_vec = _bnoSensor.read_vector(SerialBNO.Bno055VectorType.Vector_Gyroscope);
            var accel_vec = _bnoSensor.read_vector(SerialBNO.Bno055VectorType.Vector_Accelerometer);
            _bnoData.temp = _bnoSensor.read_signed_byte(SerialBNO.Bno055Registers.Bno055_Temp_Addr);
            _bnoData.gyro_x = gyro_vec.X;
            _bnoData.gyro_y = gyro_vec.Y;
            _bnoData.gyro_z = gyro_vec.Z;
            _bnoData.accel_x = accel_vec.X;
            _bnoData.accel_y = accel_vec.Y;
            _bnoData.accel_z = accel_vec.Z;
            //Debug.Print("Gyro - <" + _bnoData.gyro_x.ToString("F2") + ", "
            //            + _bnoData.gyro_y.ToString("F2") + ", "
            //            + _bnoData.gyro_z.ToString("F2") + ">\n" +
            //            "Accel - <" + _bnoData.accel_x.ToString("F2") + ", "
            //            + _bnoData.accel_y.ToString("F2") + ", "
            //            + _bnoData.accel_z.ToString("F2") + ">\n" +
            //            "Temp: " + _bnoData.temp);
        }

        public void Start() {
            _workItem.SetRepeat(true);
            FlightComputer.Instance.Execute(_workItem);
        }

        public void Stop() {
            _workItem.SetRepeat(false);
        }
    }
}