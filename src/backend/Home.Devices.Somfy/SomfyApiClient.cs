using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Home.Devices.Somfy.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Home.Devices.Somfy {

    // API documentation: https://github.com/rstrouse/ESPSomfy-RTS/wiki/Integrations

    public class SomfyApiClient : IDisposable {

        private readonly Uri _restUri;
        private readonly Uri _wsUri;

        private readonly HttpClient _http;

        public SomfyApiClient(string ip) {
            _restUri = new Uri($"http://{ip}");
            _wsUri = new Uri($"ws://{ip}:8080");
            _http = new HttpClient {
                BaseAddress = _restUri
            };
        }

        public Uri GetWsUri() {
            return _wsUri;
        }

        public async Task<string> GetBridgeVersionAsync() {
            var json = await _http.GetStringAsync("modulesettings");
            var details = JObject.Parse(json);
            return details.Value<string>("fwVersion");
        }

        public async Task<List<SomfyShadeModel>> GetShadesAsync() {
            var json = await _http.GetStringAsync("shades");
            return JsonConvert.DeserializeObject<List<SomfyShadeModel>>(json);
        }

        public async Task MoveUpAsync(int id) {
            await _http.GetStringAsync($"shadeCommand?shadeId={id}&command=up");
        }

        public async Task MoveDownAsync(int id) {
            await _http.GetStringAsync($"shadeCommand?shadeId={id}&command=down");
        }

        public async Task StopAsync(int id) {
            await _http.GetStringAsync($"shadeCommand?shadeId={id}&command=my");
        }

        public async Task MoveAsync(int id, int target) {
            await _http.GetStringAsync($"shadeCommand?shadeId={id}&target={target}");
        }

        public void Dispose() {
            _http.Dispose();
        }

    }

}
