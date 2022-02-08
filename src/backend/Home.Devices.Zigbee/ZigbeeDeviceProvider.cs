using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Home.Devices.Zigbee.Devices;
using Home.Devices.Zigbee.Models;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;

namespace Home.Devices.Zigbee {

    public class ZigbeeDeviceProvider : AbstractDeviceProvider {

        // TODO Add more logs
        // TODO Reconnect working? - make all device unreachable when disconnected
        // TODO Try/catch everywhere?

        public static ProviderDescription Descriptor = new("zigbee", ProviderDescriptionType.DeviceProvider, typeof(ZigbeeDeviceProvider), typeof(ZigbeeConfiguration));

        private readonly List<DeviceConfigurationModel> _models;
        private readonly ILogger _logger;
        private readonly ZigbeeConfiguration _configuration; 
        private readonly List<IDevice> _devices;

        private IManagedMqttClient _mqtt;

        public ZigbeeDeviceProvider(List<DeviceConfigurationModel> models, ILogger logger, IDeviceProviderConfiguration configuration) {
            _models = models;
            _logger = logger;
            _configuration = configuration as ZigbeeConfiguration;
            _devices = new List<IDevice>();
        }

        private IDevice DeviceFactory(DeviceModel model) {
            return model.Definition.Model switch {
                //"324131092621" => new ZigbeeSwitchDevice(_models, model, _mqtt, _configuration),
                //"046677552343" => new ZigbeePlugDevice(_models, model, _mqtt, _configuration),
                "WXKG11LM" => new ZigbeeButtonDevice(_models, model, _mqtt, _configuration),
                "E1812" => new ZigbeeButtonDevice(_models, model, _mqtt, _configuration),
                "WSDCGQ11LM" => new ZigbeeTemperatureDevice(_models, model, _mqtt, _configuration),
                //"SJCGQ11LM" => new ZigbeeLeakDevice(_models, model, _mqtt, _configuration),
                _ => null
            };
        }

        public override async Task ConnectAsync() {
            _logger.LogInformation($"Connecting to {_configuration.Ip}");

            var options = new ManagedMqttClientOptionsBuilder()
               .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
               .WithClientOptions(new MqttClientOptionsBuilder()
                   .WithTcpServer(_configuration.Ip)
                   .Build())
               .Build();

            _mqtt = new MqttFactory().CreateManagedMqttClient();

            await _mqtt.SubscribeAsync(
                new MqttTopicFilterBuilder().WithTopic($"{_configuration.BaseTopic}/bridge/devices").Build()
            );

            _mqtt.UseApplicationMessageReceivedHandler(async e => {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                // Debug - log all messages
                // Console.WriteLine($"Topic = {topic} - Payload = {payload}");

                if (topic == $"{_configuration.BaseTopic}/bridge/devices") {
                    // Console.WriteLine(payload);
                    var devices = JsonConvert.DeserializeObject<List<DeviceModel>>(payload);
                    _devices.Clear();
                    _devices.AddRange(devices.Where(x =>
                        x.Type != "Coordinator" &&
                        x.InterviewCompleted
                    ).Select(x => DeviceFactory(x)).Where(x => x != null).ToList());
                    NotifyObservers(_devices);
                    await _mqtt.SubscribeAsync(
                        new MqttTopicFilterBuilder().WithTopic($"{_configuration.BaseTopic}/+").Build(),
                        new MqttTopicFilterBuilder().WithTopic($"{_configuration.BaseTopic}/+/availability").Build()
                    );
                } else {
                    var topicParts = topic.Split('/');
                    if (_devices.SingleOrDefault(x => x.Name == topicParts[1]) is ZigbeeDevice device) {
                        if (topicParts.Length == 2) {
                            // Console.WriteLine($"Message {topicParts[1]}");
                            var update = JsonConvert.DeserializeObject<DeviceUpdate>(payload);
                            device.ProcessZigbeeUpdate(update);
                        } else if (topicParts.Length == 3 && topicParts[2] == "availability") {
                            // Console.WriteLine($"Availability {topicParts[1]}");
                            device.UpdateAvailability(payload == "online");
                        }
                    }
                }

            });

            _mqtt.UseConnectedHandler(e => {
                _logger.LogInformation("Connected");
            });

            await _mqtt.StartAsync(options);

        }

        public override async Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
            await _mqtt.StopAsync();
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

    }

}
