namespace RockSatC_2016.Event_Data {
    public enum EventType : byte {
        None = 0x00,
        PressureUpdate = 0x01,
        BNOUpdate100Hz = 0x02,
        GeigerUpdate = 0x03,
        BNOUpdate1Hz = 0x04
    }
}