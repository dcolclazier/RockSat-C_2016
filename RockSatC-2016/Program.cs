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
       
        public static void Main() {

            //THIS SECTION CREATES / INITIALIZES THE SERIAL LOGGER
            Debug.Print("Flight computer started successfully. Beginning INIT.");
            var baud = 115200;
            var buffer = 2048;
            var com = SerialPorts.COM1;
            Debug.Print("Initializing Serial logger on com port " + com + ", baud = " + baud + " with a max buffer of " + buffer);
            var logger = new Logger(com, baud, buffer);
            Debug.Print("Serial logger initialized.");

            //THIS SECTION CREATES/INITIALIZES THE SERIAL BNO 100HZ UPDATER
            Debug.Print("Initializing BNO Sensor on Serial Port COM4, 1 stop bit, 0 parity, 8 data bits");
            var bnoloop = new SerialBNOActions();
            bnoloop.Start();
            Debug.Print("BNO Sensor initialized.");

            //THIS SECTION CREATES/INITIALIZES THE BNO TEMPERATURE UPDATER
            //Debug.Print("About to initialize the BNO temperature update action");
            //var bnotemploop = new BNOTempUpdater();
            //Debug.Print("BNO temp update action initated.");

            //THIS SECTION CREATES/INITIALIZES THE GEIGER COUNTER UPDATER
            //Debug.Print("About to initialize the Geiger counter update action");
            //var geigerloop = new GeigerUpdater(Pins.GPIO_PIN_D2, Pins.GPIO_PIN_D3);
            //Debug.Print("Geiger action initialized.");

            Debug.Print("INIT Complete. Continuing with boot.");

            //THIS STARTS THE LOGGER
            Debug.Print("Starting logger...");
            logger.Start();
            Debug.Print("Logger started successfully.");


            //THIS STARTS THE BNO TEMP UPDATE
            //Debug.Print("Initiating BNO temp update action");
            //bnotemploop.start();
            //Debug.Print("BNO temp update action initiated.");

            //THIS STARTS THE Geiger UPDATE.
            //Debug.Print("Initating Geiger update action...");
            //geigerloop.start();
            //Debug.Print("Geiger update action initiated.");

            Debug.Print("Flight computer boot successful.");
        }

        public static void custom_delay_usec(uint time) {
            var delayStart = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
            while (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - delayStart < time*10) ;

        }
    }


}

