using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Sensors;
using RockSatC_2016.Utility;

namespace RockSatC_2016.Work_Items {
    public class BNOUpdater : Action {

        private static Bno055 _bnoSensor;
        private static I2CDevice.Configuration _bnoConfig;
        public BNOUpdater() {
            _bnoConfig = new I2CDevice.Configuration(0x28, 100);
            _bnoSensor = new Bno055(address: 0x28);

            workItem = new ThreadPool.WorkItem(TestBNO, EventType.GyroUpdate, null, true);
            FlightComputer.Instance.Execute(workItem);
        }

        private static void TestBNO()
        {

            //All default values should be 0.0 unless otherwise noted

            //print BNO calibration data
            int sysCal, gyroCal, accelCal, magCal;
            _bnoSensor.GetCalibration(out sysCal, out gyroCal, out accelCal, out magCal);
            Debug.Print("SysCal: " + sysCal + " GyroCal: " + gyroCal + " AccelCal: " + accelCal + " MagCal: " + magCal);

            //print BNO system status
            int sysStat, sysError, selfTest;
            _bnoSensor.GetSystemStatus(out sysStat, out selfTest, out sysError);
            Debug.Print("System Status: " + sysStat + " self ExampleListener: " + selfTest + " Sys error: " + sysError);

            //doesn't work... seems to respond to accelerations, but numbers wrong.
            //No default values when accel = 0, either.
            //double testAccelX = I2CBus.Instance().read16(_bnoConfig, 0x08, 0x09);
            //double testAccelY = I2CBus.Instance().read16(_bnoConfig, 0x0A, 0x0B);
            //double testAccelZ = I2CBus.Instance().read16(_bnoConfig, 0x0C, 0x0D);
            //Debug.Print("X: " + (testAccelX /= 100.0).ToString("F1") +
            //           " Y: " + (testAccelY /= 100.0).ToString("F1") +
            //           " Z: " + (testAccelZ /= 100.0).ToString("F1"));

            //kinda works, but numbers off? not sure about this one.
            //double testMagX = I2CBus.Instance().read16(_bnoConfig, 0x0E, 0x0F);
            //double testMagY = I2CBus.Instance().read16(_bnoConfig, 0x10, 0x11);
            //double testMagZ = I2CBus.Instance().read16(_bnoConfig, 0x12, 0x13);
            //Debug.Print("X: " + (testMagX /= 16.0).ToString("F1") +
            //           " Y: " + (testMagY /= 16.0).ToString("F1") +
            //           " Z: " + (testMagZ /= 16.0).ToString("F1"));


            //works
            double temp = I2CBus.Instance().read8(_bnoConfig, 0x34);
            Debug.Print("Temp: " + temp);

            Thread.Sleep(500);
        }
    }
}