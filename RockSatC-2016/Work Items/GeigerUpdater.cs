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

    public class AccelUpdater {
        private static readonly AnalogInput XPin = new AnalogInput(AnalogChannels.ANALOG_PIN_A0);
        private static readonly AnalogInput YPin = new AnalogInput(AnalogChannels.ANALOG_PIN_A1);
        private static readonly AnalogInput ZPin = new AnalogInput(AnalogChannels.ANALOG_PIN_A2);

        private readonly AccelData _accelData = new AccelData();
        private readonly ThreadPool.WorkItem _workItem;
        private readonly byte[] _dataArray;
        private readonly int _arraySize;

        public AccelUpdater(int arraySize) {
            Debug.Print("Initializing Accelerometer data updater");
            _arraySize = arraySize;
            _dataArray = new byte[_arraySize];
            _workItem = new ThreadPool.WorkItem(DumpAccelData, ref _dataArray, EventType.AccelDump, _accelData, true);
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
            }
            Debug.Print("Accel data dump complete - free mem: " + Debug.GC(true));
        }

        public void Start() {
            _workItem.Persistent = true;
            FlightComputer.Instance.Execute(_workItem);
        }
        public void Stop() {
            _workItem.Persistent = false;
        }
    }
    

    public class GeigerUpdater  {

        static readonly InterruptPort ShieldedGeiger = new InterruptPort(Pins.GPIO_PIN_D2, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);
        static readonly InterruptPort UnshieldedGeiger = new InterruptPort(Pins.GPIO_PIN_D3, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);

        private GeigerData geigerData;
        private readonly ThreadPool.WorkItem workItem;

        private int ShieldedCounts { get; set; }
        private int UnshieldedCounts { get; set; }

        public GeigerUpdater(){
            Debug.Print("Adding interrupt action for shielded geiger counter.");
            ShieldedGeiger.OnInterrupt += Shielded_Counter;

            Debug.Print("Adding interrupt action for unshielded geiger counter.");
            UnshieldedGeiger.OnInterrupt += Unshielded_Counter;

            Debug.Print("Creating Threadpool action, repeats every 5 seconds.");
            var unused = new byte[] {};
            workItem = new ThreadPool.WorkItem(GatherCounts, ref unused, EventType.GeigerUpdate, geigerData, true);
        }

        private void GatherCounts() {
            Thread.Sleep(5000);
            Debug.Print("Gathering Geiger counts data, resetting. " + Debug.GC(true));
            geigerData.shielded_geigerCount = ShieldedCounts;
            geigerData.unshielded_geigerCount = UnshieldedCounts;
            ShieldedCounts = 0;
            UnshieldedCounts = 0;
        }

        private void Shielded_Counter(uint data1, uint data2, DateTime time) {
            ShieldedCounts++;
        }
        private void Unshielded_Counter(uint data1, uint data2, DateTime time){
            UnshieldedCounts++;
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