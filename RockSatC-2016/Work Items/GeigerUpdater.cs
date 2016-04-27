using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Utility;

namespace RockSatC_2016.Work_Items {

    

    public class GeigerUpdater : Action {

        static InterruptPort geiger1;
        //static InterruptPort geiger2;

        public GeigerUpdater(Cpu.Pin shieldedGeigerInterrupt, Cpu.Pin unshieldedGeigerInterrupt) {

            //Debug.Print("Creating Interrupt Port for shielded geiger counter...");
            //var shieldedGeiger = new InterruptPort(shieldedGeigerInterrupt,true,Port.ResistorMode.PullUp,
            //    Port.InterruptMode.InterruptEdgeLevelHigh);
            //Debug.Print("Created.");

            Debug.Print("Creating Interrupt Port for shielded geiger counter...");
            geiger1 = new InterruptPort(shieldedGeigerInterrupt, true, Port.ResistorMode.PullUp,
                Port.InterruptMode.InterruptEdgeLevelLow);
            Debug.Print("Created.");


            //Debug.Print("Creating Interrupt Port for unshielded geiger counter...");
            //var unshieldedGeiger = new InterruptPort(unshieldedGeigerInterrupt, true, Port.ResistorMode.PullUp,
            //    Port.InterruptMode.InterruptEdgeLevelLow);
            //Debug.Print("Created.");

            Debug.Print("Adding interrupt action for shielded geiger counter.");
            //shieldedGeiger.OnInterrupt += new NativeEventHandler(Shielded_Counter);
            geiger1.OnInterrupt += new NativeEventHandler(Shielded_Counter);
            //Debug.Print("Adding interrupt action for unshielded geiger counter.");
            //unshieldedGeiger.OnInterrupt += Unshielded_Counter;

            workItem = new ThreadPool.WorkItem(GatherCounts, EventType.GeigerUpdate, geigerData, true);
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
            geigerData.shielded_geigerCount = shieldedCounts;
            shieldedCounts = 0;
            geigerData.unshielded_geigerCount = unshieldedCounts;
            unshieldedCounts = 0;
            
        }

        private readonly GeigerData geigerData = new GeigerData();
    }
}