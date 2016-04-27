using RockSatC_2016.Abstract;

namespace RockSatC_2016.Event_Data {
    public class BNOTempData : IEventData
    {
        public bool loggable => true;
        public double temp { get; set; }
    }
}