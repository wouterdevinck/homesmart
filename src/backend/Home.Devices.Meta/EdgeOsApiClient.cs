using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Home.Devices.Meta.Models;
using Newtonsoft.Json;

namespace Home.Devices.Meta {

    // https://github.com/wouterdevinck/edgeos/blob/main/api/main.go

    public class EdgeOsApiClient : IDisposable {

        private const string BaseUri = "http://localhost:7994/";

        private readonly HttpClient _http = new() {
            BaseAddress = new Uri(BaseUri)
        };

        public async Task<EdgeOsStatusResponseModel> GetStatus() {
            var json = await _http.GetStringAsync("status");
            return JsonConvert.DeserializeObject<EdgeOsStatusResponseModel>(json);
        }

        public async Task Reboot() {
            await _http.PostAsync("reboot", null);
        }

        public async Task SwitchBoot() {
            await _http.PostAsync("switch", null);
        }

        public async Task Upgrade(string url) {
            await _http.PostAsJsonAsync("upgrade", new EdgeOsUpgradeRequestModel {
                Url = url
            });
        }

        public void Dispose() {
            _http.Dispose();
        }

    }

}
