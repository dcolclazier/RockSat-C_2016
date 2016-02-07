namespace RockSatC_2016.Flight_Computer {
    public enum FlightComputerEventType : byte {
        None = 0x00,
        PressureUpdate = 0x01,
        GyroUpdate = 0x02
    }
}