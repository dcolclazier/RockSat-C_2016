using Microsoft.SPOT;
using RockSatC_2016.Event_Listeners;
using RockSatC_2016.Work_Items;
using SecretLabs.NETMF.Hardware.Netduino;

namespace RockSatC_2016 {

    public static class Program {
       
        public static void Main() {

           

            //THIS SECTION CREATES / INITIALIZES THE SERIAL LOGGER
            Debug.Print("Flight computer started successfully. Beginning INIT.");

            Debug.Print("Initializing Serial logger on COM1 with baudrate of 115200bps.  Max log buffer = 4096b");
            var logger = new Logger(SerialPorts.COM1, 115200, 512);

            //THIS SECTION CREATES/INITIALIZES THE SERIAL BNO 100HZ UPDATER
            Debug.Print("Initializing BNO Sensor on Serial Port COM4, 1 stop bit, 0 parity, 8 data bits");
            var bnoloop = new SerialBNOUpdater();

            //THIS SECTION CREATES/INITIALIZES THE GEIGER COUNTER UPDATER
            Debug.Print("Initializing geiger counter collection data");
            var geigerloop = new GeigerUpdater();

            //THIS SECTION CREATES/INITIALIZES THE GEIGER COUNTER UPDATER
            Debug.Print("Initializing fast accel dump collector with a size of 45kb");
            var acceldumploop = new AccelUpdater(12000, 500);

            Debug.Print("INIT Complete. Continuing with boot.");

            //THIS SECTION INITIALIZES AND STARTS THE MEMORY MONITOR
            Debug.Print("Starting memory monitor...");
            MemoryMonitor.Instance.Start(ref logger);
            

            //THIS STARTS THE LOGGER
            Debug.Print("Starting logger...");
            logger.Start();

            //THIS STARTS THE Accel dump update
            Debug.Print("Starting accel dumper...");
            acceldumploop.Start();

            //THIS STARTS THE BNO SENSOR UPDATE
            Debug.Print("Starting bno sensor updates...");
            bnoloop.Start();

            //THIS STARTS THE Geiger UPDATE.
            Debug.Print("Starting geiger counter data collection...");
            geigerloop.Start();

            Debug.Print("Flight computer boot successful.");
        }

        //public static void custom_delay_usec(uint microseconds) {
        //    var delayStart = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
        //    while (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - delayStart < microseconds*10) ;

        //}
    }


}

