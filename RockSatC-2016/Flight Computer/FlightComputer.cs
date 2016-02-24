using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Utility;

namespace RockSatC_2016.Flight_Computer {
    public class FlightComputer {
    
        private static FlightComputer _instance;
        public static FlightComputer Instance => _instance ?? (_instance = new FlightComputer());

        private FlightComputer() { }

        public void Execute(ThreadPool.WorkItem workItem) {
            ThreadPool.QueueWorkItem(workItem);
        }
        public static event EventTriggered OnEventTriggered;

        //object should be EventArgs
        public delegate void EventTriggered(EventType eventName, IEventData trigger);

        public void TriggerEvent(EventType eventType, IEventData trigger) {
            OnEventTriggered?.Invoke(eventType, trigger);
        }
    }

  
}

