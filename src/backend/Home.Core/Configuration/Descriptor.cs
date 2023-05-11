using System;

namespace Home.Core.Configuration {

    public enum DescriptorType {
        Provider,
        Automation,
        Telemetry,
        Remote
    }
    
    public class Descriptor {

        public Descriptor(string tag, Type providerType, Type configurationType, DescriptorType type) {
            Tag = tag;
            ProviderType = providerType;
            ConfigurationType = configurationType;
            Type = type;
        }

        public string Tag { get; set; }
        public DescriptorType Type { get; set; }
        public Type ProviderType { get; set; }
        public Type ConfigurationType { get; set; }

    }

}