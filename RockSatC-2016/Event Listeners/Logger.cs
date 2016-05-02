using System;
using System.Collections;
using System.IO.Ports;
using Microsoft.SPOT;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Utility;

namespace RockSatC_2016.Event_Listeners {
    public class Logger  {
        private string _buffer = "";
        private readonly int _maxBufferSize;
        private readonly Queue _pendingData = new Queue();
        private readonly SerialPort _openLogger;
        private readonly ThreadPool.WorkItem _workItem;

        public Logger(string comPort, int baud, int maxBuffer = 512) {

            _maxBufferSize = maxBuffer;
            Debug.Print("Initializing serial port...");
            _openLogger = new SerialPort(comPort, baud);
            Debug.Print("Serial port initialized... opening serial port.");
            _openLogger.Open();
            Debug.Print("Serial port opened.");

            Debug.Print("Creating logger thread and adding to pool...");
            _workItem = new ThreadPool.WorkItem(LogWorker, persistent: true);
           
        }

        private void LogWorker() {
            if (_pendingData.Count == 0) return;

            //Debug.Print("Data found to be written...");
            var packet = (QueuePacket)_pendingData.Dequeue();
            var logEntry = "";
                switch (packet.Name) {
                    case EventType.BNOUpdate:
                        var data = (BNOData)packet.EventData;
                        logEntry = "T:" + data.temp + ";" + "A:" + data.accel_x + "," + data.accel_y + "," +
                                       data.accel_z + ";" + "G:" + data.gyro_x + "," + data.gyro_y + "," + data.gyro_z + ";";
                        break;
                    case EventType.GeigerUpdate:
                        var geigerData = (GeigerData)packet.EventData;
                        logEntry = "R:" + geigerData.shielded_geigerCount + "," + geigerData.unshielded_geigerCount +
                                       ";";
                        break;
                    case EventType.None:
                        return;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(packet.Name),"Event Type not handled by logger... ");
                }
                if (_buffer.Length + logEntry.Length > _maxBufferSize) {
                    var data = System.Text.Encoding.UTF8.GetBytes(_buffer);
                    _openLogger.Write(data, 0, data.Length);
                    Debug.Print("Buffer flushed to SD Card - clearing..." + Debug.GC(true));
                    _buffer = "";
                }
                _buffer += logEntry;
        }
        
        struct QueuePacket {
            public EventType Name { get; }
            public IEventData EventData { get; }

            public QueuePacket(EventType eventName, IEventData eventData) {
                Name = eventName;
                EventData = eventData;
            }
        }

        private void OnDataFound(EventType eventName, IEventData trigger) {
            if (!trigger.loggable) return;
            //Debug.Print("Adding new data packet to queue...");
            _pendingData.Enqueue(new QueuePacket(eventName, trigger));
        }

        public void Start() {
            FlightComputer.Instance.Execute(_workItem);
            FlightComputer.OnEventTriggered += OnDataFound;
        }

        public void Stop() {
            Debug.Print("Stopping logger...");
            FlightComputer.OnEventTriggered -= OnDataFound;
        }

        public void Dispose() {
            Debug.Print("Disposing of logger...");
            FlightComputer.OnEventTriggered -= OnDataFound;
        }

        
    }
}