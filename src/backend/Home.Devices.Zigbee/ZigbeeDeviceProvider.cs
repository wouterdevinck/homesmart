using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Home.Devices.Zigbee.Devices;
using Home.Devices.Zigbee.Models;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using Newtonsoft.Json;

namespace Home.Devices.Zigbee {

    public class ZigbeeDeviceProvider : AbstractDeviceProvider {

        // TODO Add more logs
        // TODO Reconnect working? - make all device unreachable when disconnected
        // TODO Try/catch everywhere?

        public static Descriptor Descriptor = new("zigbee", typeof(ZigbeeDeviceProvider), typeof(ZigbeeConfiguration), DescriptorType.Provider);

        private readonly HomeConfigurationModel _home;
        private readonly ILogger _logger;
        private readonly ZigbeeConfiguration _configuration; 
        private readonly List<IDevice> _devices;

        private IManagedMqttClient _mqtt;

        public ZigbeeDeviceProvider(HomeConfigurationModel home, ILogger logger, IProviderConfiguration configuration) {
            _home = home;
            _logger = logger;
            _configuration = configuration as ZigbeeConfiguration;
            _devices = new List<IDevice>();
        }

        private IDevice DeviceFactory(DeviceModel model) {
            return model.Definition.Model switch {
                "324131092621" => new ZigbeeDimmerDevice(_home, model, _mqtt, _configuration),
                "046677552343" => new ZigbeePlugDevice(_home, model, _mqtt, _configuration),
                "HG08673" => new ZigbeeEnergyPlugDevice(_home, model, _mqtt, _configuration),
                "WXKG11LM" => new ZigbeeButtonDevice(_home, model, _mqtt, _configuration),
                "E1812" => new ZigbeeButtonDevice(_home, model, _mqtt, _configuration),
                "WSDCGQ11LM" => new ZigbeeTemperatureDevice(_home, model, _mqtt, _configuration),
                "SJCGQ11LM" => new ZigbeeLeakDevice(_home, model, _mqtt, _configuration),
                "BTH-RA" => new ZigbeeTrvDevice(_home, model, _mqtt, _configuration),
                "E1746" => null, // TODO Repeater device
                _ => null
            };
        }

        public override async Task ConnectAsync() {
            _logger.LogInformation($"Connecting to {_configuration.Ip}");

            var mqttFactory = new MqttFactory();

            _mqtt = mqttFactory.CreateManagedMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_configuration.Ip)
                .Build();

            var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .Build();

            _mqtt.ApplicationMessageReceivedAsync += async e => {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                // Debug - log all messages
                _logger.LogInformation($"Topic = {topic} - Payload = {payload}");

                if (topic == $"{_configuration.BaseTopic}/bridge/devices") {
                    
                    var firstTime = !_devices.Any();

                    var devices = JsonConvert.DeserializeObject<List<DeviceModel>>(payload)
                        .Where(x => x.Type != "Coordinator" && x.InterviewCompleted)
                        .Select(DeviceFactory)
                        .Where(x => x != null)
                        .ToList();

                    var newDevices = devices
                        .Where(x => _devices.All(y => y.DeviceId != x.DeviceId))
                        .ToList();

                    _logger.LogInformation($"Device list: first time = {firstTime}, {devices.Count()} devices, {newDevices.Count()} new devices");

                    // var existingDevices = devices.Where(x => _devices.Any(y => y.DeviceId == x.Id)).ToList();
                    // TODO Update existing devices? Do any changes come this way?

                    _devices.AddRange(newDevices);
                    NotifyObservers(newDevices);

                    if (firstTime) {
                        await _mqtt.SubscribeAsync(new List<MqttTopicFilter> {
                            new MqttTopicFilterBuilder().WithTopic($"{_configuration.BaseTopic}/+").Build(),
                            new MqttTopicFilterBuilder().WithTopic($"{_configuration.BaseTopic}/+/availability").Build()
                        });
                    }

                } else {
                    var topicParts = topic.Split('/');
                    if (_devices.SingleOrDefault(x => x.Name == topicParts[1]) is ZigbeeDevice device) {
                        if (topicParts.Length == 2) {
                            // Console.WriteLine($"Message {topicParts[1]}");
                            DeviceUpdate update = null;
                            try {
                                update = JsonConvert.DeserializeObject<DeviceUpdate>(payload);
                            } catch (JsonException ex) {
                                _logger.LogError($"Error parsing DeviceUpdate: {ex.Message}");
                            }
                            if (update != null) {
                                device.ProcessZigbeeUpdate(update, e.ApplicationMessage.Retain);
                            }
                        } else if (topicParts.Length == 3 && topicParts[2] == "availability") {
                            // Console.WriteLine($"Availability {topicParts[1]}");
                            device.UpdateAvailability(payload == "online");
                        }
                    }
                }
            };

            _mqtt.ConnectedAsync += e => {
                _logger.LogInformation("Connected");
                return Task.CompletedTask;
            };

            _mqtt.DisconnectedAsync += e => {
                _logger.LogInformation("Disconnected");
                return Task.CompletedTask;
            };

            await _mqtt.StartAsync(managedMqttClientOptions);

            await _mqtt.SubscribeAsync(new List<MqttTopicFilter> {
                new MqttTopicFilterBuilder().WithTopic($"{_configuration.BaseTopic}/bridge/devices").Build()
            });

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
