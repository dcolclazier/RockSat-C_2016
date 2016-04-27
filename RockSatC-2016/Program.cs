using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Event_Listeners;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Sensors;
using RockSatC_2016.Utility;
using RockSatC_2016.Work_Items;
using SecretLabs.NETMF.Hardware.Netduino;

namespace RockSatC_2016 {

    public static class Program {
        private static SerialPort serial;
       
        public static void Main() {
            //testing some serial stuff...
            //serial = new SerialPort(SerialPorts.COM1,115200,Parity.None, 8, StopBits.One);
            //serial.Open();
            //serial.DataReceived += new SerialDataReceivedEventHandler(testing_serial_receive);
            //test_serial_write("Testing initial openlog mayhem....");

            
            //var pressureloop = new PressureUpdater();
            //Debug.Print("About to initialize the BNO Sensor 100 Hz update action....");
            //var bnoloop = new BNOUpdater100Hz();
            //Debug.Print("BNO action initialized.");

            //THIS SECTION CREATES/INITIALIZES THE BNO TEMPERATURE UPDATER
            //Debug.Print("About to initialize the BNO temperature update action");
            //var bnotemploop = new BNOTempUpdater();
            //Debug.Print("BNO temp update action initated.");

            //THIS SECTION CREATES/INITIALIZES THE GEIGER COUNTER UPDATER
            Debug.Print("About to initialize the Geiger counter update action");
            var geigerloop = new GeigerUpdater(Pins.GPIO_PIN_D2, Pins.GPIO_PIN_D3);
            Debug.Print("Geiger action initialized.");

            //THIS SECTION CREATES/INITIALIZES THE SERIAL LOGGER
            var baud = 115200;
            var buffer = 512;
            var com = SerialPorts.COM1;
            Debug.Print("About to initialize Serial logger on come port " + com + ", baud = " + baud + " with a max buffer of " + buffer);
            var logger = new Logger(SerialPorts.COM1 ,115200, 128);
            Debug.Print("Serial logger initialized.");

            //THIS STARTS THE LOGGER
            Debug.Print("Initiating logger action");
            logger.start();
            Debug.Print("logger action intiiated.");
            
            //THIS STARTS THE PRESSURE UPDATE
            //Debug.Print("Intiating presser update action");
            //pressureloop.start();
            //Debug.Print("Pressure update action initiated.");

            //THIS STARTS THE BNO TEMP UPDATE
            //Debug.Print("Initiating BNO temp update action");
            //bnotemploop.start();
            //Debug.Print("BNO temp update action initiated.");

            //THIS STARTS THE 100HZ BNO Accel/Gyro update
            //Debug.Print("Intiating BNO 100Hz update action");
            //bnoloop.start();
            //Debug.Print("BNO 100Hz action initiated.");


            //THIS STARTS THE Geiger UPDATE.
            Debug.Print("Initating Geiger update action...");
            geigerloop.start();
            Debug.Print("Geiger update action initiated.");
        }

        private static void test_serial_write(string dataToWrite) {
            var bytes = Encoding.UTF8.GetBytes(dataToWrite);
            serial.Write(bytes,0,bytes.Length);
        }

        private static void testing_serial_receive(object sender, SerialDataReceivedEventArgs e) {
            System.Threading.Thread.Sleep(100);

            //create array for incoming bytes, read the bytes, then convert to string
            var bytes = new byte[serial.BytesToRead];
            serial.Read(bytes, 0, bytes.Length);
            var line = System.Text.Encoding.UTF8.GetChars(bytes).ToString();
            Debug.Print("Serial Echo: " + line);
        }

        public static void custom_delay_usec(uint time) {
            var delayStart = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
            while (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - delayStart < time*10) ;

        }
    }
}

