using System.Collections.Generic;
using System.Linq;
using Home.Core.Configuration.Models;

namespace Home.Core {

    public static class Helpers {

        // TODO move into abstractdevice? with a base constructor?

        public const string Signify = "Signify Netherlands B.V.";

        public static string HarmonizeManufacturer(this string manufacturer) {
            return manufacturer switch {
                "IKEA of Sweden" => "IKEA",
                "Philips" => "Philips",
                Signify => "Philips",
                _ => manufacturer
            };
        }

        public static string HarmonizeModel(this string model) {
            return model switch {
                "TRADFRI bulb E14 W op/ch 400lm" => "LED1649C5",
                _ => model
            };
        }

        public enum DeviceType {
            Media,
            SetTop,
            Console,
            Audio,
            Tv,
            Remote,
            Light,
            Switch,
            Outlet,
            Temperature,
            Leak,
            Hub
        }

        public static string GetTypeString(DeviceType type) {
            return type switch {
                DeviceType.Media => "media",
                DeviceType.SetTop => "settop",
                DeviceType.Console => "console",
                DeviceType.Audio => "audio",
                DeviceType.Tv => "tv",
                DeviceType.Remote => "remote",
                DeviceType.Light => "light",
                DeviceType.Switch => "switch",
                DeviceType.Outlet => "outlet",
                DeviceType.Temperature => "temperature",
                DeviceType.Leak => "leak",
                DeviceType.Hub => "hub",
                _ => "device"
            };
        }

        public static string GetFriendlyId(List<DeviceConfigurationModel> models, string deviceID) {
            return models.SingleOrDefault(x => x.DeviceId == deviceID)?.FriendlyId ?? deviceID;
        }

    }

}