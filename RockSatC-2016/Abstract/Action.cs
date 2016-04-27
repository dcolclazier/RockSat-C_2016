using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Utility;

namespace RockSatC_2016.Abstract {
    public class Action : IAction {

        protected ThreadPool.WorkItem workItem;


        public void start()
        {
            workItem.SetRepeat(true);
            //FlightComputer.Instance.Execute(workItem);
        }

        public void stop()
        {
            workItem.SetRepeat(false);
        }
    }
}