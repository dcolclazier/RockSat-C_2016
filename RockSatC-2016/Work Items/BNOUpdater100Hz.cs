using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Sensors;
using RockSatC_2016.Utility;

namespace RockSatC_2016.Work_Items {
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
}