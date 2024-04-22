using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Home.Core.Transport;
using Home.Devices.Unifi.Models;
using Home.Devices.Unifi.Notifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Home.Devices.Unifi {

    public class UnifiApiClient : IDisposable {

        private readonly HttpClient _http;
        private ClientWebSocketWrapper _ws;

        private readonly CookieContainer _cookieJar;

        private readonly string _ip;
        private readonly string _site;
        private readonly string _username;
        private readonly string _password;

        public UnifiApiClient(string ip, string site, string username, string password) {
            _ip = ip;
            _site = site;
            _username = username;
            _password = password;
            _cookieJar = new CookieContainer();
            var handler = new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                CookieContainer = _cookieJar
            };
            _http = new HttpClient(handler) {
                BaseAddress = new Uri($"https://{ip}")
            };
        }

        public async Task ConnectWebSocketAsync() {
            _ws = new ClientWebSocketWrapper(new Uri($"wss://{_ip}/proxy/network/wss/s/{_site}/events?clients=v2&critical_notifications=true"), false, _cookieJar);
            _ws.MessageArrived += (msg) => {
                var notif = JsonConvert.DeserializeObject<NotificationModel>(msg);
                if (notif.IsUnifiDevice) {

                }
            };
            _ws.ConnectionClosed += async () => {
                //_logger.LogInformation("WebSocket closed");
                //foreach (var device in _devices) {
                //    (device as SomfyDevice)?.UpdateAvailability(false);
                //}
                //await Task.Delay(5000);
                //_logger.LogInformation("WebSocket reconnecting");
                //await ConnectWebSocketAsync();
            };
            _ws.ConnectionError += (ex) => {
                //_logger.LogError($"WebSocket error - {ex.Message}");
            };
            try {
                //_logger.LogInformation("WebSocket connecting");
                await _ws.ConnectAsync();
            } catch (Exception ex) {
                //_logger.LogError($"WebSocket connectasync error - {ex.Message}");
            }
            if (_ws.IsConnected) {
                //_logger.LogInformation("WebSocket connected");
                //foreach (var device in _devices) {
                //    (device as SomfyDevice)?.UpdateAvailability(true);
                //}
            }
        }

        public async Task DisconnectWebSocketAsync() {
            await _ws.CloseAsync();
        }

        public async Task<bool> LoginAsync() {
            var result = await _http.PostAsJsonAsync("api/auth/login", new LoginRequest(_username, _password));
            var token = result.Headers.GetValues("X-Updated-Csrf-Token").SingleOrDefault();
            _http.DefaultRequestHeaders.Add("X-Csrf-Token", token);
            return result.IsSuccessStatusCode;
        }

        public async Task<DevicesResponse> GetDevicesAsync() {
            //if (!await LoginAsync()) throw new Exception("Authentication error");
            var json = await _http.GetStringAsync($"proxy/network/v2/api/site/{_site}/device?separateUnmanaged=true&includeTrafficUsage=false");
            return JsonConvert.DeserializeObject<DevicesResponse>(json);
        }

        public async Task<List<ClientModel>> GetClientsAsync() {
            //if (!await LoginAsync()) throw new Exception("Authentication error");
            var json = await _http.GetStringAsync($"proxy/network/v2/api/site/{_site}/clients/active?includeTrafficUsage=false&includeUnifiDevices=true");
            return JsonConvert.DeserializeObject<List<ClientModel>>(json);
        }

        public async Task<bool> SetSwitchPortPowerEnabledAsync(string id, int port, bool enabled) {
            // if (!await LoginAsync()) throw new Exception("Authentication error");
            var stat = await _http.GetStringAsync($"proxy/network/api/s/{_site}/stat/device");
            var statObj = JObject.Parse(stat);
            var dataObj = statObj.GetValue("data");
            var portObj = dataObj?.Children().SingleOrDefault(x => (x as JObject)?.GetValue("device_id")?.ToString() == id) as JObject;
            var poObj = portObj?.GetValue("port_overrides") as JArray;
            if (poObj == null) return false;
            var index = poObj.TakeWhile(x => (int)x["port_idx"] != port).Count();
            // TODO Handle case where port_idx is not present in port overrides yet.
            poObj[index]["poe_mode"] = enabled ? "auto" : "off";
            var requestJson = JsonConvert.SerializeObject(new JObject {
                { "port_overrides", poObj }
            });
            var url = $"proxy/network/api/s/{_site}/rest/device/{id}";
            var data = new StringContent(requestJson, Encoding.UTF8, "application/json"); 
            var res = await _http.PutAsync(url, data);
            return res.IsSuccessStatusCode;
        }

        public void Dispose() {
            _http.Dispose();
            _ws.Dispose();
        }

    }

}
