using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Sensors;
using RockSatC_2016.Utility;

//SD Logger idea
/*

    When a sensor needs to log some data, it should be able to do so in the form of an event, maybe?
    The idea is that we need to stick logging data into a queue... That way, the sensors can keep logging, keep adding to the queue, and so on
    without needing to worry about waiting on the SD card to write. The queue will be limited in size by RAM.

    The queue should then be responsible for doing the following:
        - before the data goes into the queue, it should be added to whatever the proper buffer should be
        - should there be a different buffer for each type of data? when buffer fills, enters the logging queue
        - most efficient buffer?
        - logging queue just pulls the top data packet (the buffer from before -  512 bytes?) from the queue
        

*/



namespace RockSatC_2016 {
    public static class Program {

        private static Bmp180 _bmpSensor;
        private static Bno055 _bnoSensor;
        private static I2CDevice.Configuration _bnoConfig;
        public static void Main() {
            _bmpSensor = new Bmp180(0x77);

            _bnoConfig = new I2CDevice.Configuration(0x28,100);
            _bnoSensor = new Bno055(address: 0x28);
            
            var wtf_i_want = new ThreadPool.WorkItem(TestBNO, FlightComputerEventType.GyroUpdate, null, true);
            FlightComputer.Instance.Execute(wtf_i_want);

            var test = new ThreadPool.WorkItem(TestPressure, FlightComputerEventType.None, null, true);
            FlightComputer.Instance.Execute(test);

        }
        
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
            double testAccelX = I2CBus.Instance().read16(_bnoConfig, 0x08, 0x09);
            double testAccelY = I2CBus.Instance().read16(_bnoConfig, 0x0A, 0x0B);
            double testAccelZ = I2CBus.Instance().read16(_bnoConfig, 0x0C, 0x0D);
            Debug.Print("X: " + (testAccelX /= 100.0).ToString("F1") +
                       " Y: " + (testAccelY /= 100.0).ToString("F1") +
                       " Z: " + (testAccelZ /= 100.0).ToString("F1"));

            //kinda works, but numbers off? not sure about this one.
            double testMagX = I2CBus.Instance().read16(_bnoConfig, 0x0E, 0x0F);
            double testMagY = I2CBus.Instance().read16(_bnoConfig, 0x10, 0x11);
            double testMagZ = I2CBus.Instance().read16(_bnoConfig, 0x12, 0x13);
            Debug.Print("X: " + (testMagX /= 16.0).ToString("F1") +
                       " Y: " + (testMagY /= 16.0).ToString("F1") +
                       " Z: " + (testMagZ /= 16.0).ToString("F1"));


            //works
            double temp = I2CBus.Instance().read8(_bnoConfig, 0x34);
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
            custom_delay_usec(30);
        }

        public static void custom_delay_usec(uint time) {
            var delayStart = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
            while ((Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - delayStart) < time*10) ;

        }

       
    }
}

