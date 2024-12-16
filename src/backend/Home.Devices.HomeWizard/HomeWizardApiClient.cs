using System;
using System.Net.Http;
using System.Threading.Tasks;
using Home.Devices.HomeWizard.Models;
using Newtonsoft.Json;

namespace Home.Devices.HomeWizard {

    public class HomeWizardApiClient(string ip) : IDisposable {

        private readonly HttpClient _http = new() {
            BaseAddress = new Uri($"http://{ip}/api/")
        };

        public async Task<DeviceModel> GetStatus() {
            var json = await _http.GetStringAsync("");
            return JsonConvert.DeserializeObject<DeviceModel>(json);
        }

        public async Task<T> GetData<T>() {
            var json = await _http.GetStringAsync("v1/data");
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Dispose() {
            _http.Dispose();
        }

    }

}
