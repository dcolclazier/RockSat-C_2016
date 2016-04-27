using System;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Utility;

namespace RockSatC_2016.Work_Items {
    public class GeigerUpdater : Action {
        public GeigerUpdater(Cpu.Pin shieldedGeigerInterrupt, Cpu.Pin unshieldedGeigerInterrupt) {
            var shieldedGeiger = new InterruptPort(shieldedGeigerInterrupt,true,Port.ResistorMode.PullUp,
                Port.InterruptMode.InterruptEdgeLevelLow);
            var unshieldedGeiger = new InterruptPort(unshieldedGeigerInterrupt, true, Port.ResistorMode.PullUp,
                Port.InterruptMode.InterruptEdgeLevelLow);

            shieldedGeiger.OnInterrupt += Shielded_Counter;
            unshieldedGeiger.OnInterrupt += Unshielded_Counter;

            workItem = new ThreadPool.WorkItem(GatherCounts, EventType.GeigerUpdate, geigerData, true);
        }

        private void Shielded_Counter(uint data1, uint data2, DateTime time) {
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