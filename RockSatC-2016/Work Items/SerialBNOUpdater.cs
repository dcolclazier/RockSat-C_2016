using Microsoft.SPOT;
using RockSatC_2016.Abstract;
using RockSatC_2016.Drivers;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Utility;
using SecretLabs.NETMF.Hardware.Netduino;

namespace RockSatC_2016.Work_Items {
    public class SerialBNOUpdater {

        private readonly SerialBNO _bnoSensor;

        private BNOData _bnoData;

        private readonly WorkItem _workItem;

        public SerialBNOUpdater() {
            _bnoSensor = new SerialBNO(SerialPorts.COM4,5000,5000,SerialBNO.Bno055OpMode.Operation_Mode_Ndof);
            _bnoData = new BNOData();
            var unused = new byte[] {};
            _workItem = new WorkItem(GyroUpdater, ref unused, EventType.BNOUpdate, _bnoData, true, true);

            _bnoSensor.begin();
        }

        private void GyroUpdater() {
            
            var gyro_vec = _bnoSensor.read_vector(SerialBNO.Bno055VectorType.Vector_Gyroscope);
            _bnoData.gyro_x = gyro_vec.X;
            _bnoData.gyro_y = gyro_vec.Y;
            _bnoData.gyro_z = gyro_vec.Z;

            var accel_vec = _bnoSensor.read_vector(SerialBNO.Bno055VectorType.Vector_Accelerometer);
            _bnoData.accel_x = accel_vec.X;
            _bnoData.accel_y = accel_vec.Y;
            _bnoData.accel_z = accel_vec.Z;

            _bnoData.temp = _bnoSensor.read_signed_byte(SerialBNO.Bno055Registers.Bno055_Temp_Addr);
            //Debug.Print("Gyro - <" + _bnoData.gyro_x.ToString("F2") + ", "
            //            + _bnoData.gyro_y.ToString("F2") + ", "
            //            + _bnoData.gyro_z.ToString("F2") + ">\n" +
            //            "Accel - <" + _bnoData.accel_x.ToString("F2") + ", "
            //            + _bnoData.accel_y.ToString("F2") + ", "
            //            + _bnoData.accel_z.ToString("F2") + ">\n" +
            //            "Temp: " + _bnoData.temp);
            Debug.Print("BNO Sensor update complete.");
        }

        public void Start() {
            _workItem.Start();
        }
        
    }
}