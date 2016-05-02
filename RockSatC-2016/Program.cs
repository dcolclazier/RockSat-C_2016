using Microsoft.SPOT;
using RockSatC_2016.Event_Listeners;
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

            //THIS SECTION CREATES/INITIALIZES THE SERIAL BNO 100HZ UPDATER
            Debug.Print("Initializing BNO Sensor on Serial Port COM4, 1 stop bit, 0 parity, 8 data bits");
            var bnoloop = new SerialBNOActions();

            //THIS SECTION CREATES/INITIALIZES THE GEIGER COUNTER UPDATER
            //Debug.Print("Initializing geiger counter collection data");
            //var geigerloop = new GeigerUpdater();

            Debug.Print("INIT Complete. Continuing with boot.");

            //THIS STARTS THE LOGGER
            Debug.Print("Starting logger...");
            logger.Start();

            //THIS STARTS THE BNO SENSOR UPDATE
            Debug.Print("Starting bno sensor updates...");
            bnoloop.Start();

            //THIS STARTS THE Geiger UPDATE.
            //Debug.Print("Starting geiger counter data collection...");
            //geigerloop.start();

            Debug.Print("Flight computer boot successful.");
        }

        public static void custom_delay_usec(uint microseconds) {
            var delayStart = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
            while (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - delayStart < microseconds*10) ;

        }
    }


}

