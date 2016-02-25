using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using RockSatC_2016.Utility;

namespace RockSatC_2016.Sensors {
    public class Bno055 {

        private readonly I2CDevice.Configuration _slaveConfig;

        public enum Bno055Registers : byte {
            /* Page id register definition */
            Bno055_Page_Id_Addr = 0X07,

            /* PAGE0 REGISTER DEFINITION START*/
            Bno055_Chip_Id_Addr = 0x00,
            Bno055_Accel_Rev_Id_Addr = 0x01,
            Bno055_Mag_Rev_Id_Addr = 0x02,
            Bno055_Gyro_Rev_Id_Addr = 0x03,
            Bno055_Sw_Rev_Id_Lsb_Addr = 0x04,
            Bno055_Sw_Rev_Id_Msb_Addr = 0x05,
            Bno055_Bl_Rev_Id_Addr = 0X06,

            /* Accel data register */
            Bno055_Accel_Data_X_Lsb_Addr = 0X08,
            Bno055_Accel_Data_X_Msb_Addr = 0X09,
            Bno055_Accel_Data_Y_Lsb_Addr = 0X0A,
            Bno055_Accel_Data_Y_Msb_Addr = 0X0B,
            Bno055_Accel_Data_Z_Lsb_Addr = 0X0C,
            Bno055_Accel_Data_Z_Msb_Addr = 0X0D,

            /* Mag data register */
            Bno055_Mag_Data_X_Lsb_Addr = 0X0E,
            Bno055_Mag_Data_X_Msb_Addr = 0X0F,
            Bno055_Mag_Data_Y_Lsb_Addr = 0X10,
            Bno055_Mag_Data_Y_Msb_Addr = 0X11,
            Bno055_Mag_Data_Z_Lsb_Addr = 0X12,
            Bno055_Mag_Data_Z_Msb_Addr = 0X13,

            /* Gyro data registers */
            Bno055_Gyro_Data_X_Lsb_Addr = 0X14,
            Bno055_Gyro_Data_X_Msb_Addr = 0X15,
            Bno055_Gyro_Data_Y_Lsb_Addr = 0X16,
            Bno055_Gyro_Data_Y_Msb_Addr = 0X17,
            Bno055_Gyro_Data_Z_Lsb_Addr = 0X18,
            Bno055_Gyro_Data_Z_Msb_Addr = 0X19,

            /* Euler data registers */
            Bno055_Euler_H_Lsb_Addr = 0X1A,
            Bno055_Euler_H_Msb_Addr = 0X1B,
            Bno055_Euler_R_Lsb_Addr = 0X1C,
            Bno055_Euler_R_Msb_Addr = 0X1D,
            Bno055_Euler_P_Lsb_Addr = 0X1E,
            Bno055_Euler_P_Msb_Addr = 0X1F,

            /* Quaternion data registers */
            Bno055_Quaternion_Data_W_Lsb_Addr = 0X20,
            Bno055_Quaternion_Data_W_Msb_Addr = 0X21,
            Bno055_Quaternion_Data_X_Lsb_Addr = 0X22,
            Bno055_Quaternion_Data_X_Msb_Addr = 0X23,
            Bno055_Quaternion_Data_Y_Lsb_Addr = 0X24,
            Bno055_Quaternion_Data_Y_Msb_Addr = 0X25,
            Bno055_Quaternion_Data_Z_Lsb_Addr = 0X26,
            Bno055_Quaternion_Data_Z_Msb_Addr = 0X27,

            /* Linear acceleration data registers */
            Bno055_Linear_Accel_Data_X_Lsb_Addr = 0X28,
            Bno055_Linear_Accel_Data_X_Msb_Addr = 0X29,
            Bno055_Linear_Accel_Data_Y_Lsb_Addr = 0X2A,
            Bno055_Linear_Accel_Data_Y_Msb_Addr = 0X2B,
            Bno055_Linear_Accel_Data_Z_Lsb_Addr = 0X2C,
            Bno055_Linear_Accel_Data_Z_Msb_Addr = 0X2D,

            /* Gravity data registers */
            Bno055_Gravity_Data_X_Lsb_Addr = 0X2E,
            Bno055_Gravity_Data_X_Msb_Addr = 0X2F,
            Bno055_Gravity_Data_Y_Lsb_Addr = 0X30,
            Bno055_Gravity_Data_Y_Msb_Addr = 0X31,
            Bno055_Gravity_Data_Z_Lsb_Addr = 0X32,
            Bno055_Gravity_Data_Z_Msb_Addr = 0X33,

            /* Temperature data register */
            Bno055_Temp_Addr = 0X34,

            /* Status registers */
            Bno055_Calib_Stat_Addr = 0X35,
            Bno055_Selftest_Result_Addr = 0X36,
            Bno055_Intr_Stat_Addr = 0X37,

            Bno055_Sys_Clk_Stat_Addr = 0X38,
            Bno055_Sys_Stat_Addr = 0X39,
            Bno055_Sys_Err_Addr = 0X3A,

            /* Unit selection register */
            Bno055_Unit_Sel_Addr = 0X3B,
            Bno055_Data_Select_Addr = 0X3C,

            /* Mode registers */
            Bno055_Opr_Mode_Addr = 0X3D,
            Bno055_Pwr_Mode_Addr = 0X3E,

            Bno055_Sys_Trigger_Addr = 0X3F,
            Bno055_Temp_Source_Addr = 0X40,

            /* Axis remap registers */
            Bno055_Axis_Map_Config_Addr = 0X41,
            Bno055_Axis_Map_Sign_Addr = 0X42,

            /* SIC registers */
            Bno055_Sic_Matrix_0_Lsb_Addr = 0X43,
            Bno055_Sic_Matrix_0_Msb_Addr = 0X44,
            Bno055_Sic_Matrix_1_Lsb_Addr = 0X45,
            Bno055_Sic_Matrix_1_Msb_Addr = 0X46,
            Bno055_Sic_Matrix_2_Lsb_Addr = 0X47,
            Bno055_Sic_Matrix_2_Msb_Addr = 0X48,
            Bno055_Sic_Matrix_3_Lsb_Addr = 0X49,
            Bno055_Sic_Matrix_3_Msb_Addr = 0X4A,
            Bno055_Sic_Matrix_4_Lsb_Addr = 0X4B,
            Bno055_Sic_Matrix_4_Msb_Addr = 0X4C,
            Bno055_Sic_Matrix_5_Lsb_Addr = 0X4D,
            Bno055_Sic_Matrix_5_Msb_Addr = 0X4E,
            Bno055_Sic_Matrix_6_Lsb_Addr = 0X4F,
            Bno055_Sic_Matrix_6_Msb_Addr = 0X50,
            Bno055_Sic_Matrix_7_Lsb_Addr = 0X51,
            Bno055_Sic_Matrix_7_Msb_Addr = 0X52,
            Bno055_Sic_Matrix_8_Lsb_Addr = 0X53,
            Bno055_Sic_Matrix_8_Msb_Addr = 0X54,

            /* Accelerometer Offset registers */
            Accel_Offset_X_Lsb_Addr = 0X55,
            Accel_Offset_X_Msb_Addr = 0X56,
            Accel_Offset_Y_Lsb_Addr = 0X57,
            Accel_Offset_Y_Msb_Addr = 0X58,
            Accel_Offset_Z_Lsb_Addr = 0X59,
            Accel_Offset_Z_Msb_Addr = 0X5A,

            /* Magnetometer Offset registers */
            Mag_Offset_X_Lsb_Addr = 0X5B,
            Mag_Offset_X_Msb_Addr = 0X5C,
            Mag_Offset_Y_Lsb_Addr = 0X5D,
            Mag_Offset_Y_Msb_Addr = 0X5E,
            Mag_Offset_Z_Lsb_Addr = 0X5F,
            Mag_Offset_Z_Msb_Addr = 0X60,

            /* Gyroscope Offset register s*/
            Gyro_Offset_X_Lsb_Addr = 0X61,
            Gyro_Offset_X_Msb_Addr = 0X62,
            Gyro_Offset_Y_Lsb_Addr = 0X63,
            Gyro_Offset_Y_Msb_Addr = 0X64,
            Gyro_Offset_Z_Lsb_Addr = 0X65,
            Gyro_Offset_Z_Msb_Addr = 0X66,

            /* Radius registers */
            Accel_Radius_Lsb_Addr = 0X67,
            Accel_Radius_Msb_Addr = 0X68,
            Mag_Radius_Lsb_Addr = 0X69,
            Mag_Radius_Msb_Addr = 0X6A
        }

        private enum Bno055PowerMode : byte {
            Power_Mode_Normal = 0X00,
            Power_Mode_Lowpower = 0X01,
            Power_Mode_Suspend = 0X02
        }

        public enum Bno055OpMode : byte {
            /* Operation mode settings*/
            Operation_Mode_Config = 0X00,
            Operation_Mode_Acconly = 0X01,
            Operation_Mode_Magonly = 0X02,
            Operation_Mode_Gyronly = 0X03,
            Operation_Mode_Accmag = 0X04,
            Operation_Mode_Accgyro = 0X05,
            Operation_Mode_Maggyro = 0X06,
            Operation_Mode_Amg = 0X07,
            Operation_Mode_Imuplus = 0X08,
            Operation_Mode_Compass = 0X09,
            Operation_Mode_M4G = 0X0A,
            Operation_Mode_Ndof_Fmc_Off = 0X0B,
            Operation_Mode_Ndof = 0X0C
        }

        public struct Bno055RevInfo {
            public uint AccelRev;
            public uint MagRev;
            public uint GyroRev;
            public uint SwRev;
            public uint BlRev;
        }

        public enum Bno055VectorType {
            Vector_Accelerometer = Bno055Registers.Bno055_Accel_Data_X_Lsb_Addr,
            Vector_Magnetometer = Bno055Registers.Bno055_Mag_Data_X_Lsb_Addr,
            Vector_Gyroscope = Bno055Registers.Bno055_Gyro_Data_X_Lsb_Addr,
            Vector_Euler = Bno055Registers.Bno055_Euler_H_Lsb_Addr,
            Vector_Linearaccel = Bno055Registers.Bno055_Linear_Accel_Data_X_Lsb_Addr,
            Vector_Gravity = Bno055Registers.Bno055_Gravity_Data_X_Lsb_Addr
        }

        //public Bno055(byte address = 0x28, int clockKHz = 100, Bno055OpMode mode = Bno055OpMode.Operation_Mode_Ndof) {
        //    _slaveConfig = new I2CDevice.Configuration(address, clockKHz);
        //    Address = address;

        //    while (!Init(mode))
        //    {
        //        Debug.Print("9dof sensor not detected...");
        //    }
        //    SetExtCrystalUse(true);
        //}
        public Bno055(I2CDevice.Configuration slaveConfig, Bno055OpMode mode = Bno055OpMode.Operation_Mode_Ndof) {
            _slaveConfig = slaveConfig;
            Address = slaveConfig.Address;

            while (!Init(mode))
            {
                Debug.Print("9dof sensor not detected...");
            }
            SetExtCrystalUse(true);
        }

        public ushort Address { get; }

        public bool Init(Bno055OpMode mode = Bno055OpMode.Operation_Mode_Ndof) {
            //var id = Read8(Bno055Registers.Bno055_Chip_Id_Addr);
            var id = I2CBus.Instance().read8(_slaveConfig,(byte)Bno055Registers.Bno055_Chip_Id_Addr);
            if (id != _bno055Id) {
                Debug.Print("We didn't get the right chip address, waiting then trying again.");
                Thread.Sleep(500);
                id = Read8(Bno055Registers.Bno055_Chip_Id_Addr);
                if (id != _bno055Id) return false;
            }
            else {
                Debug.Print("We read the Chip ID Address!");
            }

            SetMode(Bno055OpMode.Operation_Mode_Config);
            Write8(Bno055Registers.Bno055_Sys_Trigger_Addr, 0x20);
            while (Read8(Bno055Registers.Bno055_Chip_Id_Addr) != _bno055Id) {
                Thread.Sleep(10);
            }
            Thread.Sleep(300);

            Write8(Bno055Registers.Bno055_Pwr_Mode_Addr, (byte) Bno055PowerMode.Power_Mode_Normal);
            Thread.Sleep(10);

            Write8(Bno055Registers.Bno055_Page_Id_Addr, 0);

            Write8(Bno055Registers.Bno055_Sys_Trigger_Addr, 0x0);
            Thread.Sleep(10);

            SetMode(mode);
            Thread.Sleep(20);
            return true;
        }

        public void SetMode(Bno055OpMode mode) {
            _mode = mode;
            Write8(Bno055Registers.Bno055_Opr_Mode_Addr, (byte) _mode);
        }

        public Bno055RevInfo GetRevInfo() {
            var info = new Bno055RevInfo {
                AccelRev = Read8(Bno055Registers.Bno055_Accel_Rev_Id_Addr),
                MagRev = Read8(Bno055Registers.Bno055_Mag_Rev_Id_Addr),
                GyroRev = Read8(Bno055Registers.Bno055_Gyro_Rev_Id_Addr),
                BlRev = Read8(Bno055Registers.Bno055_Bl_Rev_Id_Addr)
            };

            uint lsb = Read8(Bno055Registers.Bno055_Sw_Rev_Id_Lsb_Addr);
            uint msb = Read8(Bno055Registers.Bno055_Sw_Rev_Id_Msb_Addr);
            info.SwRev = (((msb) << 8) | (lsb));
            return info;
        }

        public void SetExtCrystalUse(bool usextal) {
            var modeback = _mode;

            SetMode(Bno055OpMode.Operation_Mode_Config);
            Thread.Sleep(25);
            Write8(Bno055Registers.Bno055_Page_Id_Addr, 0);
            Write8(Bno055Registers.Bno055_Sys_Trigger_Addr, usextal ? (byte)0x80 : (byte)0x00);
            Thread.Sleep(10);
            /* Set the requested operating mode (see section 3.3) */
            SetMode(modeback);
            Thread.Sleep(20);
        
        }

        public void GetSystemStatus(out int systemStatus, out int selfTestResult, out int systemError) {
            Write8(Bno055Registers.Bno055_Page_Id_Addr, 0);
            systemStatus = Read8(Bno055Registers.Bno055_Sys_Stat_Addr);
            systemError = Read8(Bno055Registers.Bno055_Sys_Err_Addr);
            selfTestResult = Read8(Bno055Registers.Bno055_Selftest_Result_Addr);
            Thread.Sleep(200);
        }

        public void GetCalibration(out int system, out int gyro, out int accel, out int mag) {
            int calData = Read8(Bno055Registers.Bno055_Calib_Stat_Addr);
             system = (calData >> 6) & 0x03;
             gyro = (calData >> 4) & 0x03;
             accel = (calData >> 2) & 0x03;
             mag = calData & 0x03;
        }
       
        public Vector GetVector(Bno055VectorType vectorType) {
            var xyz = new Vector();

            var buffer = ReadLen((Bno055Registers)vectorType, 6);

            var x = buffer[0] | (buffer[1] << 8);
            var y = buffer[2] | (buffer[3] << 8);
            var z = buffer[4] | (buffer[5] << 8);
            switch (vectorType)
            {
                case Bno055VectorType.Vector_Magnetometer:
                    /* 1uT = 16 LSB */
                    xyz.X = x / 16.0;
                    xyz.Y = y / 16.0;
                    xyz.Z = z / 16.0;
                    break;
                case Bno055VectorType.Vector_Gyroscope:
                    /* 1rps = 900 LSB */
                    xyz.X = x / 900.0;
                    xyz.Y = y / 900.0;
                    xyz.Z = z / 900.0;
                    break;
                case Bno055VectorType.Vector_Euler:
                    /* 1 degree = 16 LSB */
                    xyz.X = x / 16.0;
                    xyz.Y = y / 16.0;
                    xyz.Z = z / 16.0;
                    break;
                case Bno055VectorType.Vector_Accelerometer:
                case Bno055VectorType.Vector_Linearaccel:
                case Bno055VectorType.Vector_Gravity:
                    /* 1m/s^2 = 100 LSB */
                    xyz.X = x / 100.0;
                    xyz.Y = y / 100.0;
                    xyz.Z = z / 100.0;
                    break;
            }

            return xyz;
        }

        public Quaternion GetQuat() {

            /* Read quat data (8 bytes) */
            var buffer = ReadLen(Bno055Registers.Bno055_Quaternion_Data_W_Lsb_Addr, 8);
            var w = ((buffer[1]) << 8) | (buffer[0]);
            var x = ((buffer[3]) << 8) | (buffer[2]);
            var y = ((buffer[5]) << 8) | (buffer[4]);
            var z = ((buffer[7]) << 8) | (buffer[6]);

            /* Assign to Quaternion */
            /* See http://ae-bst.resource.bosch.com/media/products/dokumente/bno055/BST_BNO055_DS000_12~1.pdf
               3.6.5.5 Orientation (Quaternion)  */
            const double scale = (1.0 / (1 << 14));
            return new Quaternion(scale * w, scale* x, scale* y, scale* z);
        }

        public int GetTemp() {
            return Read8(Bno055Registers.Bno055_Temp_Addr);
        }

        //public bool getEvent() {
            
        //}//sensors_event_t*

        //public void getSensor() {
            
        //}//sensor_t*

        private byte Read8(Bno055Registers reg) {
            byte[] buffer = new byte[1];
            I2CBus.Instance().ReadRegister(_slaveConfig,(byte)reg,buffer,1000);
            return buffer[0];
        }

      

        private byte[] ReadLen(Bno055Registers reg, int length) {
            var buffer = new byte[length];
            I2CBus.Instance().Read(_slaveConfig,buffer);
            return buffer;
        }

        private bool Write8(Bno055Registers reg, byte value) {
            I2CBus.Instance().WriteRegister(_slaveConfig,(byte)reg,value,1000);
            return true;
        }

        private Bno055OpMode _mode;
        private byte _bno055Id = 0xA0;
    }
}