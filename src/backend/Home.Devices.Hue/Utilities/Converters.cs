namespace Home.Devices.Hue.Utilities {

    internal static class Converters {

        public static byte MapToByte(this double value) {
            return (byte)(value * 254 / 100);
        }

        public static double MapFromByte(this byte value) {
            return value * 100 / 254;
        }

    }

}
