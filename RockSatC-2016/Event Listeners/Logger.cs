using System;
using System.Collections;
using System.IO.Ports;
using Microsoft.SPOT;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Utility;
using RockSatC_2016.Work_Items;

namespace RockSatC_2016.Event_Listeners {
    public class Logger  {
        private string buffer = "";
        private readonly int maxBufferSize;
        private readonly Queue dataToBeWrittenTo = new Queue();
        private readonly SerialPort open_logger;
        private readonly ThreadPool.WorkItem workItem;

        public Logger(string comPort, int baud, int maxBuffer = 512) {

            maxBufferSize = maxBuffer;
            Debug.Print("Initializing serial port...");
            open_logger = new SerialPort(comPort, baud);
            Debug.Print("Serial port initialized... opening serial port.");
            open_logger.Open();
            Debug.Print("Serial port opened.");

            Debug.Print("Creating logger thread and adding to pool...");
            workItem = new ThreadPool.WorkItem(LogWorker, isPersistent: true);
           
        }

        private void LogWorker() {
            if (dataToBeWrittenTo.Count == 0) return;

            //Debug.Print("Data found to be written...");
            var packet = dataToBeWrittenTo.Dequeue() as QueuePacket;
            var logEntry = "";
            if (packet != null) { 
                switch (packet.Name) {
                    case EventType.BNOUpdate:
                        //Debug.Print("Logger received BNO Update");
                        var data = packet.EventData as BNOData;
                        if (data != null)
                            logEntry = "T:" + data.temp + ";" + "A:" + data.accel_x + "," + data.accel_y + "," +
                                       data.accel_z + ";" + "G:" + data.gyro_x + "," + data.gyro_y + "," + data.gyro_z + ";";
                        //Debug.Print("Data log entry updated.");
                        break;
                    case EventType.GeigerUpdate:
                        //Debug.Print("Logger received Geiger Update");
                        var geigerData = packet.EventData as GeigerData;
                        if (geigerData != null)
                            logEntry = "R:" + geigerData.shielded_geigerCount + "," + geigerData.unshielded_geigerCount +
                                       ";";
                        //Debug.Print("Data added to buffer.");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(packet.Name),"Event Type not handled by logger... ");
                }
                if (buffer.Length + logEntry.Length > maxBufferSize) {
                    var data = System.Text.Encoding.UTF8.GetBytes(buffer);
                    open_logger.Write(data, 0, data.Length);
                    Debug.Print("Buffer flushed to SD Card - clearing..." + Debug.GC(true));
                    buffer = "";
                }
                buffer += logEntry;
            }
        }
        
        class QueuePacket {
            public EventType Name { get; private set; }
            public IEventData EventData { get; private set; }

            public QueuePacket(EventType eventName, IEventData eventData) {
                Name = eventName;
                EventData = eventData;
            }
        }

        

        public void OnDataFound(EventType eventName, IEventData trigger) {
            if (!trigger.loggable) return;
            //Debug.Print("Adding new data packet to queue...");
            dataToBeWrittenTo.Enqueue(new QueuePacket(eventName, trigger));
        }


        public void Start() {
            FlightComputer.Instance.Execute(workItem);
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