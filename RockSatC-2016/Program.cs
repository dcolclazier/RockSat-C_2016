using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
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

            var pressureloop = new PressureUpdater();
            var bnoloop = new BNOUpdater();
            var logger = new Logger(SerialPorts.COM1 ,115200, 128);


            pressureloop.start();
            bnoloop.start();
            logger.start();

        }
        public static void custom_delay_usec(uint time) {
            var delayStart = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
            while ((Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - delayStart) < time*10) ;

        }
    }
}

