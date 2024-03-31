using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Home.Devices.Unifi.Models;
using Newtonsoft.Json;

namespace Home.Devices.Unifi {

    public class UnifiApiClient : IDisposable {

        private readonly HttpClient _http;
        private readonly string _site;
        private readonly string _username;
        private readonly string _password;

        public UnifiApiClient(string ip, string site, string username, string password) {
            _site = site;
            _username = username;
            _password = password;
            var handler = new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            _http = new HttpClient(handler) {
                BaseAddress = new Uri($"https://{ip}"),
            };
            // string wss = $"wss://{ip}/proxy/network/wss/s/{site}/events?clients=v2&critical_notifications=true";
        }

        private async Task<bool> LoginAsync() {
            var result = await _http.PostAsJsonAsync("api/auth/login", new LoginRequest(_username, _password));
            return result.IsSuccessStatusCode;
        }

        public async Task<DevicesResponse> GetDevicesAsync() {
            if (!await LoginAsync()) {
                throw new Exception("Authentication error");
            }
            var json = await _http.GetStringAsync($"proxy/network/v2/api/site/{_site}/device?separateUnmanaged=true&includeTrafficUsage=false");
            return JsonConvert.DeserializeObject<DevicesResponse>(json);
        }

        public void Dispose() {
            _http.Dispose();
        }

    }

}
