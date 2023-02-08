using System.Collections.Generic;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Devices;
using Microsoft.Extensions.Logging;

namespace Home.Automations {

    public class PushOnOffAutomation : AbstractDeviceConsumer {

        public static Descriptor Descriptor = new("pushOnOff", typeof(PushOnOffAutomation), typeof(PushOnOffAutomationConfiguration), DescriptorType.DeviceConsumer, DescriptorSubtype.Automation);

        public override string Type => "Toggle Button";

        private readonly ILogger _logger;
        private readonly PushOnOffAutomationConfiguration _configuration;

        public PushOnOffAutomation(ILogger logger, PushOnOffAutomationConfiguration configuration)
        : base(new List<string> { configuration.PushButtonId, configuration.OnOffDeviceId }) {
            _logger = logger;
            _configuration = configuration;
        }

        protected override void Start() {
            _logger.LogInformation("Automation starting");
            var source = Devices[_configuration.PushButtonId] as IPushButton;
            var target = Devices[_configuration.OnOffDeviceId] as IOnOffDevice;
            if (source != null && target != null) {
                source.SinglePress += async (s, e) => {
                    _logger.LogInformation($"Automation: pushbutton {source.DeviceId} toggling {target.DeviceId}");
                    await target.ToggleOnOffAsync();
                };
            } else {
                _logger.LogError("Automation failed to start");
            }
        }

    }

    public class PushOnOffAutomationConfiguration : IDeviceConsumerConfiguration {

        public string PushButtonId { get; set; }
        public string OnOffDeviceId { get; set; }

    }

}
