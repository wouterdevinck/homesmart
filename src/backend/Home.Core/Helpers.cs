using System;
using System.Reflection;

namespace Home.Core {

    public static class Helpers {

        // TODO move into abstractdevice? with a base constructor?

        public const string Signify = "Signify Netherlands B.V.";
        public const string Jaga = "Jaga";
        public const string Somfy = "Somfy";
        public const string Ubiquiti = "Ubiquiti";
        public const string HomeWizard = "HomeWizard";
        public const string Diy = "DIY";

        public const string VersionNotAvailable = "n/a";

        public static string HarmonizeManufacturer(this string manufacturer) {
            return manufacturer switch {
                "IKEA of Sweden" => "IKEA",
                "Philips" => "Philips",
                Signify => "Philips",
                "Aqara" => "Xiaomi",
                "_TZ3210_ttkgurpb" => "Miboxer",
                Jaga => Jaga,
                Somfy => Somfy,
                Ubiquiti => Ubiquiti,
                Diy => Diy,
                _ => manufacturer
            };
        }

        public static string HarmonizeModel(this string model) {
            return model switch {
                "TRADFRI bulb E14 W op/ch 400lm" => "LED1649C5", // IKEA LED1649C5
                "TS0504B" => "FUT038Z", // Miboxer FUT038Z
                _ => model
            };
        }

        public enum DeviceType {
            Light,
            Switch,
            Outlet,
            Temperature,
            Leak,
            Hub,
            Solar,
            Fancoil,
            Trv,
            Shutter,
            Camera,
            NetworkAccessPoint,
            NetworkSwitch,
            Watermeter
        }

        public static string GetTypeString(DeviceType type) {
            return type switch {
                DeviceType.Light => "light",
                DeviceType.Switch => "switch",
                DeviceType.Outlet => "outlet",
                DeviceType.Temperature => "temperature",
                DeviceType.Leak => "leak",
                DeviceType.Hub => "hub",
                DeviceType.Solar => "solar",
                DeviceType.Fancoil => "fancoil",
                DeviceType.Trv => "trv",
                DeviceType.Shutter => "shutter",
                DeviceType.Camera => "camera",
                DeviceType.NetworkAccessPoint => "wifi",
                DeviceType.NetworkSwitch => "networkswitch",
                DeviceType.Watermeter => "watermeter",
                _ => "device"
            };
        }

        public static T ConvertType<T>(object value) {
            if (typeof(T).GetTypeInfo().IsEnum) {
                return (T)Enum.Parse(typeof(T), value.ToString() ?? string.Empty, true);
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }

    }

}