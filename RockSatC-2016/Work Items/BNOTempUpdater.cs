using System.Threading;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Sensors;
using RockSatC_2016.Utility;

namespace RockSatC_2016.Work_Items {
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