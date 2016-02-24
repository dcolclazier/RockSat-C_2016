using System;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;

namespace RockSatC_2016.Abstract {
    interface IEventListener : IDisposable {
        void MyTrigger(EventType eventName, IEventData trigger);
        void Start();
        void Stop();
    }
}