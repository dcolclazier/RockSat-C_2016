using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;

namespace RockSatC_2016.Event_Listeners {
    public class ExampleListener :  IEventListener
    {

        public void MyTrigger(EventType eventName, object trigger) {

            //This listens for Gyro data!!
            if (eventName != EventType.GyroUpdate) return;
            
            //log data!

            //otherwise do stuff with gyro data thats stored inside of trigger
        }

        public void MyTrigger(EventType eventName, IEventData trigger) {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            FlightComputer.OnEventTriggered += MyTrigger;
        }

        public void Stop()
        {
            FlightComputer.OnEventTriggered -= MyTrigger;
        }

        public void Dispose() {
            FlightComputer.OnEventTriggered -= MyTrigger;
        }
    }
}