using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Devices.Tuya.Api;
using Home.Devices.Tuya.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SuperSimpleTcp;

namespace Home.Devices.Tuya {

    [Device]
    public abstract partial class TuyaDevice : AbstractDevice {
        
        private const int Port = 6668;
        private const int RetryPeriod = 10000;
        private const int HeartbeatInterval = 10000;

        private readonly TuyaDeviceConfiguration _model;
        private readonly SimpleTcpClient _tcp;
        private readonly ILogger _logger;
        private readonly Timer _heartbeatTimer;

        protected TuyaDevice(HomeConfigurationModel home, ILogger logger, TuyaDeviceConfiguration model) : base(home, $"TUYA-{model.Id}") {
            _model = model;
            _logger = logger;
            _tcp = new SimpleTcpClient($"{model.Ip}:{Port}");
            _tcp.Keepalive.EnableTcpKeepAlives = true;
            _tcp.Keepalive.TcpKeepAliveInterval = 5;
            _tcp.Keepalive.TcpKeepAliveTime = 5;
            _tcp.Keepalive.TcpKeepAliveRetryCount = 5;
            _tcp.Events.Connected += async (s, e) => await Connected(s, e);
            _tcp.Events.Disconnected += Disconnected;
            _tcp.Events.DataReceived += DataReceived;
            _heartbeatTimer = new Timer(HeartbeatInterval);
            _heartbeatTimer.Elapsed += OnHeartbeat;
            _heartbeatTimer.AutoReset = true;
            _heartbeatTimer.Enabled = false;
        }

        public async Task ConnectAsync() {
            await Task.Run(() => {
                try {
                    _tcp.ConnectWithRetries(RetryPeriod);
                } catch (Exception) {
                    _logger.LogWarning("Failed to connect");
                    _heartbeatTimer.Enabled = true;
                }
            });
        }

        public async Task DisconnectAsync() {
            _heartbeatTimer.Enabled = false;
            await _tcp.DisconnectAsync();
        }

        private async Task Connected(object sender, ConnectionEventArgs e) {
            _logger.LogInformation($"Connected to {e.IpPort}");
            _heartbeatTimer.Enabled = true;
            await SendCommand(TuyaCommand.DpQuery, new TuyaRequest(_model.Id));
        }

        private void Disconnected(object sender, ConnectionEventArgs e) {
            UpdateAvailability(false);
            _logger.LogWarning($"Device {e.IpPort} disconnected");
        }

        private void DataReceived(object sender, DataReceivedEventArgs e) {
            // TODO Take framing properly into account - doesn't seem to be an issue
            var res = TuyaParser.DecodeResponse(e.Data.Array, Encoding.UTF8.GetBytes(_model.Key));
            _logger.LogDebug($"Received {e.Data.Count} bytes from {e.IpPort}: {res.Command} {res.ReturnCode} {res.Json}");
            if (res.ReturnCode == 0 && !string.IsNullOrWhiteSpace(res.Json)) {
                UpdateAvailability(true);
                if (res.Command is TuyaCommand.Status or TuyaCommand.DpQuery) {
                    try {
                        var dps = JsonConvert.DeserializeObject<TuyaDps>(res.Json);
                        ProcessDps(dps);
                    } catch (Exception ex) {
                        _logger.LogError($"Exception while processing DP's: {ex.Message}");
                    }
                }
            }
        }

        protected abstract void ProcessDps(TuyaDps dps);
        protected abstract void UpdateAvailability(bool available);

        private async void OnHeartbeat(object source, ElapsedEventArgs e) {
            var mustConnect = false;
            if (!_tcp.IsConnected) {
                mustConnect = true;
            } else {
                try {
                    var req = new TuyaRequest(_model.Id);
                    await SendCommand(TuyaCommand.HeartBeat, req);
                } catch (IOException) {
                    _logger.LogWarning("Failed to send heartbeat");
                    mustConnect = true;
                }
            }
            if (mustConnect) {
                UpdateAvailability(false);
                await ConnectAsync();
            }
        }
        
        protected async Task SetDpAsync(int dp, object value) {
            var dps = new TuyaDpsRequest(_model.Id, dp, value);
            await SendCommand(TuyaCommand.Control, dps);
        }

        private async Task SendCommand(TuyaCommand command, TuyaRequest request) {
            try {
                var json = JsonConvert.SerializeObject(request);
                var req = TuyaParser.EncodeRequest(command, json, Encoding.UTF8.GetBytes(_model.Key));
                await _tcp.SendAsync(req);
            } catch (Exception ex) {
                _logger.LogError($"Error while sending command - {ex.Message}");
            }
        }

    }

}