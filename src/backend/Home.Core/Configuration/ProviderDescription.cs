using System;

namespace Home.Core.Configuration {

    public enum ProviderDescriptionType {
        DeviceProvider,
        Automation,
        Telemetry
    }

    public class ProviderDescription {

        public ProviderDescription(string tag, ProviderDescriptionType type, Type providerType, Type configurationType) {
            Tag = tag;
            Type = type;
            ProviderType = providerType;
            ConfigurationType = configurationType;
        }

        public string Tag { get; set; }
        public ProviderDescriptionType Type { get; set; }
        public Type ProviderType { get; set; }
        public Type ConfigurationType { get; set; }

    }

}