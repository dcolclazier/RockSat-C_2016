namespace RockSatC_2016.Flight_Computer {
    public class Test
    {
        Test()
        {
            FlightComputer.Instance.OnEventTriggered += MyTrigger;
            
        }

        void MyTrigger(FlightComputerEventType eventName, object trigger) {

            if (eventName != FlightComputerEventType.PressureUpdate) return;

            //log data!

            //otherwise do stuff with gyro data thats stored inside of trigger
        }
    }
}