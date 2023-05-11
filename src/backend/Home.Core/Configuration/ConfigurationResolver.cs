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
            if (currentType == typeof(IProviderConfiguration)) {
                type = GetConfigurationTypeByTagAndType((_previousNodeEvent as Scalar)?.Value, DescriptorType.Provider);
            } else if (currentType == typeof(ITelemetryConfiguration)) {
                type = GetConfigurationTypeByTagAndType((_previousNodeEvent as Scalar)?.Value, DescriptorType.Telemetry);
            } else if (currentType == typeof(IRemoteConfiguration)) {
                type = GetConfigurationTypeByTagAndType((_previousNodeEvent as Scalar)?.Value, DescriptorType.Remote);
            } else if (currentType == typeof(ConfigurationWithDescriptionModel<IAutomationConfiguration>)) {
                _currentDeviceConsumerType = (_previousNodeEvent as Scalar)?.Value;
            } else if (currentType == typeof(IAutomationConfiguration)) {
                type = GetConfigurationTypeByTagAndType(_currentDeviceConsumerType, DescriptorType.Automation);
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
