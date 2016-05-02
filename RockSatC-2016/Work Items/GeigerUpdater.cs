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

    

    public class GeigerUpdater  {

        static readonly InterruptPort shieldedGeiger = new InterruptPort(Pins.GPIO_PIN_D2, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);
        static readonly InterruptPort unshieldedGeiger = new InterruptPort(Pins.GPIO_PIN_D3, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);

        private GeigerData geigerData;
        private readonly ThreadPool.WorkItem workItem;

        private int shieldedCounts { get; set; }
        private int unshieldedCounts { get; set; }

        public GeigerUpdater(){
            Debug.Print("Adding interrupt action for shielded geiger counter.");
            shieldedGeiger.OnInterrupt += Shielded_Counter;

            Debug.Print("Adding interrupt action for unshielded geiger counter.");
            unshieldedGeiger.OnInterrupt += Unshielded_Counter;

            Debug.Print("Creating Threadpool action, repeats every 5 seconds.");
            workItem = new ThreadPool.WorkItem(GatherCounts, EventType.GeigerUpdate, geigerData, true);
        }

        private void GatherCounts() {
            Thread.Sleep(5000);
            Debug.Print("Gathering Geiger counts data, resetting.");
            geigerData.shielded_geigerCount = shieldedCounts;
            geigerData.unshielded_geigerCount = unshieldedCounts;
            shieldedCounts = 0;
            unshieldedCounts = 0;
        }

        private void Shielded_Counter(uint data1, uint data2, DateTime time) {
            shieldedCounts++;
        }
        private void Unshielded_Counter(uint data1, uint data2, DateTime time){
            unshieldedCounts++;
        }

        public void Start() {
            workItem.Persistent = true;
            FlightComputer.Instance.Execute(workItem);
        }

        public void Stop() {
            workItem.Persistent = false;
        }
    }
}