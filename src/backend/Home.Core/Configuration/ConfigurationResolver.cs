using System;
using System.Collections.Generic;
using System.Linq;
using Home.Core.Configuration.Models;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Home.Core.Configuration {

    public class ConfigurationResolver : INodeTypeResolver {

        private NodeEvent _previousNodeEvent;
        private string _currentAutomationType;

        private List<ProviderDescription> _providerDescriptors;

        public ConfigurationResolver(List<ProviderDescription> providerDescriptors) {
            _providerDescriptors = providerDescriptors;
        }

        public bool Resolve(NodeEvent nodeEvent, ref Type currentType) {
            Type type = null;
            if (currentType == typeof(IDeviceProviderConfiguration)) {
                var tag = (_previousNodeEvent as Scalar)?.Value;
                type = GetConfigurationTypeByTagAndType(tag, ProviderDescriptionType.DeviceProvider);
            } else if (currentType == typeof(AutomationConfigurationModel)) {
                _currentAutomationType = (_previousNodeEvent as Scalar)?.Value;
            } else if (currentType == typeof(IAutomationConfiguration)) {
                type = GetConfigurationTypeByTagAndType(_currentAutomationType, ProviderDescriptionType.Automation);
            } else if (currentType == typeof(ITelemetryConfiguration)) {
                var tag = (_previousNodeEvent as Scalar)?.Value;
                type = GetConfigurationTypeByTagAndType(tag, ProviderDescriptionType.Telemetry);
            }
            if (type != null) {
                currentType = type;
                return true;
            }
            _previousNodeEvent = nodeEvent;
            return false;
        }

        private Type GetConfigurationTypeByTagAndType(string tag, ProviderDescriptionType type) {
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
