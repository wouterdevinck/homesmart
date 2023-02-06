using System;

namespace Home.Core.Configuration {

    public enum DescriptorType {
        DeviceProvider,
        DeviceConsumer
    }

    public enum DescriptorSubtype {
        None,
        Automation,
        Telemetry
    }

    public class Descriptor {

        public Descriptor(string tag, Type providerType, Type configurationType, DescriptorType type, DescriptorSubtype subtype = DescriptorSubtype.None) {
            Tag = tag;
            ProviderType = providerType;
            ConfigurationType = configurationType;
            Type = type;
            Subttype = subtype;
        }

        public string Tag { get; set; }
        public DescriptorType Type { get; set; }
        public DescriptorSubtype Subttype { get; set; }
        public Type ProviderType { get; set; }
        public Type ConfigurationType { get; set; }

    }

}