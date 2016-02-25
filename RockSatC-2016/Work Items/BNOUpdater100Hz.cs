using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Sensors;
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

    internal class GeigerData : IEventData {
        public bool loggable => true;
        public int shielded_geigerCount { get; set; }
        public int unshielded_geigerCount { get; set; }
    }
   

    public class BNOUpdater100Hz : Action {

        private readonly Bno055 _bnoSensor;
        private readonly BNOData _bnoData;
        private readonly I2CDevice.Configuration _bnoConfig;
        public BNOUpdater100Hz() {
            _bnoConfig = new I2CDevice.Configuration(0x28, 100);
            _bnoSensor = new Bno055(_bnoConfig);
            _bnoData = new BNOData();
            workItem = new ThreadPool.WorkItem(GryoUpdater, EventType.BNOUpdate100Hz, null, true);
            FlightComputer.Instance.Execute(workItem);
        }

        private void GryoUpdater() {


            _bnoData.gyro_x = I2CBus.Instance().read16(_bnoConfig, 0x14, 0x15);
            _bnoData.gyro_y = I2CBus.Instance().read16(_bnoConfig, 0x16, 0x17);
            _bnoData.gyro_z = I2CBus.Instance().read16(_bnoConfig, 0x18, 0x19);
            _bnoData.accel_x = I2CBus.Instance().read16(_bnoConfig, 0x28, 0x29);
            _bnoData.accel_y = I2CBus.Instance().read16(_bnoConfig, 0x2A, 0x2B);
            _bnoData.accel_z = I2CBus.Instance().read16(_bnoConfig, 0x2C, 0x2D);

            Thread.Sleep(10);
        }
    }
    public class BNOTempUpdater : Action {

        private readonly Bno055 _bnoSensor;
        private readonly BNOTempData _bnoTempData;
        private readonly I2CDevice.Configuration _bnoConfig;
        public BNOTempUpdater() {
            _bnoConfig = new I2CDevice.Configuration(0x28, 100);
            _bnoSensor = new Bno055(_bnoConfig);
            _bnoTempData = new BNOTempData();
            workItem = new ThreadPool.WorkItem(TempUpdater, EventType.BNOUpdate1Hz, null, true);
            FlightComputer.Instance.Execute(workItem);
        }

        private void TempUpdater() {


            _bnoTempData.temp = I2CBus.Instance().read8(_bnoConfig, 0x34);


            Thread.Sleep(1000);
        }
    }
}