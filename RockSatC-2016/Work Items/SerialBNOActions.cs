using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Sensors;
using RockSatC_2016.Utility;
using SecretLabs.NETMF.Hardware.Netduino;

namespace RockSatC_2016.Work_Items {
    public class SerialBNOActions {

        private readonly SerialBNO _bnoSensor;

        private readonly BNOData _bnoData;

        private readonly ThreadPool.WorkItem workItem;

        public SerialBNOActions() {
            _bnoSensor = new SerialBNO(SerialPorts.COM4,5000,5000,SerialBNO.Bno055OpMode.Operation_Mode_Ndof);
            _bnoData = new BNOData();

            workItem = new ThreadPool.WorkItem(GyroUpdater, EventType.BNOUpdate, _bnoData, true);

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
            FlightComputer.Instance.Execute(workItem);
        }

        public void Stop() {
            throw new System.NotImplementedException();
        }
    }

    //public class BNOUpdater100Hz : Action {

    //    private readonly Bno055 _bnoSensor;
    //    private readonly BNOGyroData _bnoGyroData;
    //    private readonly I2CDevice.Configuration _bnoConfig;
    //    public BNOUpdater100Hz() {
    //        _bnoConfig = new I2CDevice.Configuration(0x28, 100);
    //        _bnoSensor = new Bno055(_bnoConfig);
    //        _bnoGyroData = new BNOGyroData();
    //        workItem = new ThreadPool.WorkItem(GryoUpdater, EventType.BNOUpdate100Hz, null, true);
    //        FlightComputer.Instance.Execute(workItem);
    //    }

    //    private void GryoUpdater() {


    //        _bnoGyroData.gyro_x = I2CBus.Instance().read16(_bnoConfig, 0x14, 0x15);
    //        _bnoGyroData.gyro_y = I2CBus.Instance().read16(_bnoConfig, 0x16, 0x17);
    //        _bnoGyroData.gyro_z = I2CBus.Instance().read16(_bnoConfig, 0x18, 0x19);
    //        _bnoGyroData.accel_x = I2CBus.Instance().read16(_bnoConfig, 0x28, 0x29);
    //        _bnoGyroData.accel_y = I2CBus.Instance().read16(_bnoConfig, 0x2A, 0x2B);
    //        _bnoGyroData.accel_z = I2CBus.Instance().read16(_bnoConfig, 0x2C, 0x2D);

    //        Thread.Sleep(10);
    //    }
    //}
}