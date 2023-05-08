using System;
using System.Collections.Generic;
using System.Linq;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Home.Core.Configuration {

    public class ConfigurationResolver : INodeTypeResolver {

        private NodeEvent _previousNodeEvent;
        private string _currentDeviceConsumerType;

        private readonly List<Descriptor> _providerDescriptors;

        public ConfigurationResolver(List<Descriptor> providerDescriptors) {
            _providerDescriptors = providerDescriptors;
        }

        public bool Resolve(NodeEvent nodeEvent, ref Type currentType) {
            Type type = null;
            if (currentType == typeof(IDeviceProviderConfiguration)) {
                var tag = (_previousNodeEvent as Scalar)?.Value;
                type = GetConfigurationTypeByTagAndType(tag, DescriptorType.DeviceProvider);
            } else if (currentType == typeof(ConfigurationWithDescriptionModel<IDeviceConsumerConfiguration>)) {
                _currentDeviceConsumerType = (_previousNodeEvent as Scalar)?.Value;
            } else if (currentType == typeof(IDeviceConsumerConfiguration)) {
                type = GetConfigurationTypeByTagAndType(_currentDeviceConsumerType, DescriptorType.DeviceConsumer);
            }
            if (type != null) {
                currentType = type;
                return true;
            }
            _previousNodeEvent = nodeEvent;
            return false;
        }

        private Type GetConfigurationTypeByTagAndType(string tag, DescriptorType type) {
            if (!string.IsNullOrEmpty(tag)) {
                var providerDescriptor = _providerDescriptors.SingleOrDefault(x => x.Tag == tag && x.Type == type);
                if (providerDescriptor != null) {
                    return providerDescriptor.ConfigurationType;
                }
            }
            return null;
        }

    }

}
