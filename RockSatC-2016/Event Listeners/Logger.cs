using System;
using System.Collections;
using System.IO.Ports;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Utility;

namespace RockSatC_2016.Event_Listeners {
    public class Logger : Action, IEventListener {
        private string buffer = "";
        private readonly int maxBufferSize;
        private readonly Queue dataToBeWrittenTo = new Queue();
        private readonly SerialPort open_logger;

        public Logger(string comPort, int baud, int maxBuffer = 512) {

            maxBufferSize = maxBuffer;
            open_logger = new SerialPort(comPort, baud);

            workItem = new ThreadPool.WorkItem(LogWorker, isPersistent: true);
            FlightComputer.Instance.Execute(workItem);
        }

        private void LogWorker() {
            if (dataToBeWrittenTo.Count == 0) return;

            var packet = dataToBeWrittenTo.Dequeue() as QueuePacket;
            var logEntry = "";
            if (packet != null) { 
                switch (packet.Name) {

                    case EventType.PressureUpdate:
                        var pressureData = packet.EventData as PressureData;
                        if (pressureData != null)
                            logEntry = "P:" + pressureData.Pressure + ":" + pressureData.Altitude + ":" +
                                       pressureData.Temp + ";";
                        break;
                    case EventType.GyroUpdate:
                        var gyroData = packet.EventData as BnoData;
                        if (gyroData != null) logEntry = "G:" + gyroData.Gyro_X + ":" + gyroData.Gyro_Y + ":" + gyroData.Gyro_Z + ";";
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
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
        }

        public void Stop() {
            FlightComputer.OnEventTriggered -= MyTrigger;
        }

        public void Dispose() {
            FlightComputer.OnEventTriggered -= MyTrigger;
        }
    }
}