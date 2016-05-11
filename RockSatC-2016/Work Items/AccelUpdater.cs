using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Utility;
using SecretLabs.NETMF.Hardware.Netduino;

namespace RockSatC_2016.Work_Items
{
    public class AccelUpdater  {
        private static readonly AnalogInput XPin = new AnalogInput(AnalogChannels.ANALOG_PIN_A0);
        private static readonly AnalogInput YPin = new AnalogInput(AnalogChannels.ANALOG_PIN_A1);
        private static readonly AnalogInput ZPin = new AnalogInput(AnalogChannels.ANALOG_PIN_A2);

        private readonly AccelData _accelData = new AccelData();
        private readonly WorkItem _workItem;
        private readonly byte[] _dataArray;
        private readonly int _arraySize;
        private int _frequency;

        public AccelUpdater(int arraySize, int frequency) {
            Debug.Print("Initializing Accelerometer data updater");
            _arraySize = arraySize;
            _dataArray = new byte[_arraySize];
            _workItem = new WorkItem(DumpAccelData, ref _dataArray, EventType.AccelDump, _accelData, persistent:true, pauseable:true);
            _frequency = frequency;
        }

        private void DumpAccelData()
        {
            short x = 0;
            for (var i = 0; i < _arraySize; i+=2)
            {
                short raw = 0;
                switch (x++%3) {
                    case 0: raw = (short)(ZPin.Read() * 1000);
                        break;
                    case 2: raw = (short)(YPin.Read() * 1000);
                        break;
                    case 1: raw = (short)(XPin.Read() * 1000);
                        break;
                }

                _dataArray[i] = (byte) (raw >> 8);
                _dataArray[i + 1] = (byte) (raw & 255);
                //var period = 1000*(1/_frequency);
                //if (period < 1) period = 1;
                //Thread.Sleep(period);
            }
            Debug.Print("Accel data dump complete - free mem: " + Debug.GC(true));
        }

        public void Start() {
            _workItem.Start();
        }
        
    }
}