namespace RockSatC_2016.Flight_Computer {
    public class FlightComputer {

        private static FlightComputer _instance;
        public static FlightComputer Instance => _instance ?? (_instance = new FlightComputer());

        private FlightComputer() { }

        public void Execute(ThreadPool.WorkItem workItem) {
            ThreadPool.QueueWorkItem(workItem);
        }
        public event EventTriggered OnEventTriggered;

        //object should be EventArgs
        public delegate void EventTriggered(FlightComputerEventType eventName, object trigger);

        public void TriggerEvent(FlightComputerEventType eventType, object trigger) {
            OnEventTriggered?.Invoke(eventType, trigger);
        }
    }
}