using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;
using RockSatC_2016.Sensors;
using RockSatC_2016.Utility;

namespace RockSatC_2016.Work_Items {
    //public class PressureUpdater : Action {

    //    private static Bmp180 _bmpSensor;
        
    //    private readonly PressureData pressureData = new PressureData();
    //    public PressureUpdater() {
           
    //        workItem = new ThreadPool.WorkItem(TestPressure, 
    //            EventType.PressureUpdate, eventData:pressureData, isPersistent:true);
    //        _bmpSensor = new Bmp180(0x77);
    //    }
    //    private void TestPressure()
    //    {
    //        //works
    //        pressureData.Pressure = _bmpSensor.GetPressure() / 100.0F;
    //        pressureData.Temp = _bmpSensor.GetTemperature();
    //        pressureData.Altitude = Bmp180.PressureToAltitude( Bmp180.SensorsPressureSealevelhpa, 
    //                                                    pressureData.Pressure, pressureData.Temp);
    //        Program.custom_delay_usec(30);
    //    }

     
    //}
}