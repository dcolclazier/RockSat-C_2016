namespace RockSatC_2016.Event_Data {
    public enum EventType : byte {
        None = 0x00,
        PressureUpdate = 0x01,
        GyroUpdate = 0x02,
        Temp = 0x03
    }
}