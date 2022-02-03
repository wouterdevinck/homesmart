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
            if (currentType == typeof(IDeviceProviderConfiguration)) {
                var tag = (_previousNodeEvent as Scalar)?.Value;
                if (!string.IsNullOrEmpty(tag)) {
                    var providerDescriptor = _providerDescriptors.SingleOrDefault(x => x.Tag == tag && x.Type == ProviderDescriptionType.DeviceProvider);
                    if (providerDescriptor != null) {
                        currentType = providerDescriptor.ConfigurationType;
                        return true;
                    }
                }
            } else if (currentType == typeof(AutomationConfigurationModel)) {
                _currentAutomationType = (_previousNodeEvent as Scalar)?.Value;
            } else if (currentType == typeof(IAutomationConfiguration)) {
                if (!string.IsNullOrEmpty(_currentAutomationType)) {
                    var providerDescriptor = _providerDescriptors.SingleOrDefault(x => x.Tag == _currentAutomationType && x.Type == ProviderDescriptionType.Automation);
                    if (providerDescriptor != null) {
                        currentType = providerDescriptor.ConfigurationType;
                        return true;
                    }
                }
            } 
            _previousNodeEvent = nodeEvent;
            return false;
        }

    }

}
