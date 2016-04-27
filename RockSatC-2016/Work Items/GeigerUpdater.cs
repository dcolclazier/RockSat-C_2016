using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Utility;
using SecretLabs.NETMF.Hardware.Netduino;

namespace RockSatC_2016.Work_Items {

    

    public class GeigerUpdater : Action {

        static readonly InterruptPort shieldedGeiger = new InterruptPort(Pins.GPIO_PIN_D2, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
        static readonly InterruptPort unshieldedGeiger = new InterruptPort(Pins.GPIO_PIN_D3, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
        //static InterruptPort geiger2;

        public GeigerUpdater(Cpu.Pin shieldedGeigerInterrupt, Cpu.Pin unshieldedGeigerInterrupt) {

            
           Debug.Print("Adding interrupt action for shielded geiger counter.");
            //shieldedGeiger.OnInterrupt += new NativeEventHandler(Shielded_Counter);
            shieldedGeiger.OnInterrupt += Shielded_Counter;
            //Debug.Print("Adding interrupt action for unshielded geiger counter.");
            unshieldedGeiger.OnInterrupt += Unshielded_Counter;

            workItem = new ThreadPool.WorkItem(GatherCounts, EventType.GeigerUpdate, geigerData, true);
            FlightComputer.Instance.Execute(workItem);
        }

        private void Shielded_Counter(UInt32 data1, UInt32 data2, DateTime time) {
            shieldedCounts++;
        }
        private void Unshielded_Counter(uint data1, uint data2, DateTime time) {
            unshieldedCounts++;
        }

        private int shieldedCounts { get; set; }
        private int unshieldedCounts { get; set; }

        private void GatherCounts() {
            Thread.Sleep(5000);
            Debug.Print("Gathering Geiger Data Counts.");
            geigerData.shielded_geigerCount = shieldedCounts;
            shieldedCounts = 0;
            geigerData.unshielded_geigerCount = unshieldedCounts;
            unshieldedCounts = 0;
        }

        private readonly GeigerData geigerData = new GeigerData();
    }
}