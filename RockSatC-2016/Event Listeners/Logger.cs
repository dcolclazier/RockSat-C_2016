using System;
using System.Collections;
using System.IO.Ports;
using Microsoft.SPOT;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Utility;
using RockSatC_2016.Work_Items;
using IEventListener = RockSatC_2016.Abstract.IEventListener;

namespace RockSatC_2016.Event_Listeners {
    public class Logger : Action, IEventListener {
        private string buffer = "";
        private readonly int maxBufferSize;
        private readonly Queue dataToBeWrittenTo = new Queue();
        private readonly SerialPort open_logger;

        public Logger(string comPort, int baud, int maxBuffer = 512) {

            maxBufferSize = maxBuffer;
            Debug.Print("Initializing serial port...");
            open_logger = new SerialPort(comPort, baud);
            Debug.Print("Serial port initialized... opening serial port.");
            open_logger.Open();
            Debug.Print("Serial port opened.");

            Debug.Print("Adding Logger thread to threadpool...");
            workItem = new ThreadPool.WorkItem(LogWorker, isPersistent: true);
            Debug.Print("Executing logger thread...");
            FlightComputer.Instance.Execute(workItem);
        }

        private void LogWorker() {
            if (dataToBeWrittenTo.Count == 0) return;

            Debug.Print("Data found to be written...");
            var packet = dataToBeWrittenTo.Dequeue() as QueuePacket;
            var logEntry = "";
            if (packet != null) { 
                switch (packet.Name) {
                    case EventType.BNOUpdate1Hz:
                        Debug.Print("Data determined to be BNO Temp Update");
                        var tempData = packet.EventData as BNOTempData;
                        if (tempData != null)
                            logEntry = "T:" + tempData.temp + ";";
                        Debug.Print("Data log entry updated.");
                        break;
                    case EventType.GeigerUpdate:
                        Debug.Print("Data determined to be Geiger Update");
                        var geigerData = packet.EventData as GeigerData;
                        if (geigerData != null)
                            logEntry = "G:" + geigerData.shielded_geigerCount + ":" + geigerData.unshielded_geigerCount +
                                       ";";
                        Debug.Print("Data log entry updated.");
                        break;
                    case EventType.PressureUpdate:
                        Debug.Print("Data determined to be Pressure Update");
                        var pressureData = packet.EventData as PressureData;
                        if (pressureData != null)
                            logEntry = "P:" + pressureData.Pressure + ":" + pressureData.Altitude + ":" +
                                       pressureData.Temp + ";";
                        Debug.Print("Data log entry updated.");
                        break;
                    case EventType.BNOUpdate100Hz:
                        Debug.Print("Data determined to be BNO Accel Update");
                        var bnoData = packet.EventData as BNOData;
                        if (bnoData != null) logEntry = "B:" + bnoData.gyro_x + ":" + bnoData.gyro_y + ":" + bnoData.gyro_z 
                                                        + bnoData.accel_x + ":" + bnoData.accel_y + ":" + bnoData.accel_z + ";";
                        Debug.Print("Data log entry updated.");
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(packet.Name),"Event Type not handled by logger... ");
                }
                if (buffer.Length + logEntry.Length > maxBufferSize) FlushBufferToSD();
                buffer += logEntry;
            }
        }

        private void FlushBufferToSD() {
            var data = System.Text.Encoding.UTF8.GetBytes(buffer);
            open_logger.Write(data,0,data.Length);
            buffer = "";
        }


        class QueuePacket {
            public EventType Name { get; private set; }
            public IEventData EventData { get; private set; }

            public QueuePacket(EventType eventName, IEventData eventData) {
                Name = eventName;
                EventData = eventData;
            }
        }

        

        public void MyTrigger(EventType eventName, IEventData trigger) {
            if (!trigger.loggable) return;

            dataToBeWrittenTo.Enqueue(new QueuePacket(eventName, trigger));
        }


        public void Start() {
            FlightComputer.OnEventTriggered += MyTrigger;
            start();
        }

        public void Stop() {
            FlightComputer.OnEventTriggered -= MyTrigger;
        }

        public void Dispose() {
            FlightComputer.OnEventTriggered -= MyTrigger;
        }
    }
}