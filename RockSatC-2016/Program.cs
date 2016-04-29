using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Sensors;
using RockSatC_2016.Utility;

namespace RockSatC_2016 {
    public static class Program {

        private static Bmp180 _bmpSensor;
        private static Bno055 _bnoSensor;
        //private static I2CDevice.Configuration _bnoConfig;
        public static void Main() {

            //_bnoConfig = new I2CDevice.Configuration(0x28,100);
            //ultralowpower, ~45hz
            //standard, ~35hz
            _bmpSensor = new Bmp180(0x77);
            while (!_bmpSensor.Init(Bmp180.Mode.Bmp085_Mode_Ultralowpower))
            {
                Debug.Print("BMP sensor not detected...");
            }

            _bnoSensor = new Bno055(address: 0x28);
            while (!_bnoSensor.Init())
            {
                Debug.Print("9dof sensor not detected...");
            }
            _bnoSensor.SetExtCrystalUse(true);

            var bnoTest = new ThreadPool.WorkItem(TestBNO, FlightComputerEventType.GyroUpdate, null, true);
            FlightComputer.Instance.Execute(bnoTest);

            var test = new ThreadPool.WorkItem(TestPressure, FlightComputerEventType.PressureUpdate,null,true);
            FlightComputer.Instance.Execute(test);

            

        }

        //private static int read16(byte lsbRegister, byte msbRegister) {
        //    var lsb = new byte[1];
        //    I2CBus.Instance().ReadRegister(_bnoConfig, lsbRegister, lsb, 1000);
        //    var msb = new byte[1];
        //    I2CBus.Instance().ReadRegister(_bnoConfig, msbRegister, msb, 1000);
        //    var test = (((msb[0]) << 8) | (lsb[0]));
        //    return test;
        //}
        //private static byte read8(byte reg)
        //{
        //    byte[] buffer = new byte[1];
        //    I2CBus.Instance().ReadRegister(_bnoConfig, reg, buffer, 1000);
        //    return buffer[0];
        //}
        private static void TestBNO() {

            //All default values should be 0.0 unless otherwise noted

            //print BNO calibration data
            int sysCal, gyroCal, accelCal, magCal;
            _bnoSensor.GetCalibration(out sysCal, out gyroCal, out accelCal, out magCal);
            Debug.Print("SysCal: " + sysCal + " GyroCal: " + gyroCal + " AccelCal: " + accelCal + " MagCal: " + magCal);

            //print BNO system status
            int sysStat, sysError, selfTest;
            _bnoSensor.GetSystemStatus(out sysStat,out selfTest, out sysError);
            Debug.Print("System Status: " + sysStat + " self Test: " + selfTest + " Sys error: " + sysError);

            //doesn't work... seems to respond to accelerations, but numbers wrong.
            //No default values when accel = 0, either.
            Vector accelVector = _bnoSensor.GetVector(Bno055.Bno055VectorType.Vector_Accelerometer);
            //double testAccelX = read16(0x08, 0x09);
            //double testAccelY = read16(0x0A, 0x0B);
            //double testAccelZ = read16(0x0C, 0x0D);
            Debug.Print("X: " + (accelVector.X /= 100.0).ToString("F1") +
                       " Y: " + (accelVector.Y /= 100.0).ToString("F1") +
                       " Z: " + (accelVector.Z /= 100.0).ToString("F1"));

            //kinda works, but numbers off? not sure about this one.
            var magVector = _bnoSensor.GetVector(Bno055.Bno055VectorType.Vector_Magnetometer);

            //double testMagX = read16(0x0E, 0x0F);
            //double testMagY = read16(0x10, 0x11);
            //double testMagZ = read16(0x12, 0x13);
            Debug.Print("X: " + (magVector.X /= 16.0).ToString("F1") +
                       " Y: " + (magVector.Y /= 16.0).ToString("F1") +
                       " Z: " + (magVector.Z /= 16.0).ToString("F1"));


            //works
            var temp = _bnoSensor.GetTemp();
            Debug.Print("Temp: " + temp);

            Thread.Sleep(500);
        }

        private static void TestPressure()
        {
            //works
            var pressure = _bmpSensor.GetPressure() / 100.0F;
            var temp = _bmpSensor.GetTemperature();
            Debug.Print("Seconds: " + (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks / 10000000)
                + "\n" + "Pressure:    " + pressure + " hPa"
                + "\n" + "Temperature: " + temp + " C"
                + "\n" + "Altitude:    "
                + Bmp180.PressureToAltitude(Bmp180.SensorsPressureSealevelhpa, pressure, temp) + " m" + "\n");
        }

        public static void custom_delay_usec(uint time) {
            var delayStart = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
            while ((Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - delayStart) < time*10) ;

        }

       
    }
}

