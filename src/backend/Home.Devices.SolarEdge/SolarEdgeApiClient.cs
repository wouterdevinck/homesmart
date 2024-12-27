using System;
using System.Net.Http;
using System.Threading.Tasks;
using Home.Devices.SolarEdge.Models;
using Newtonsoft.Json;

namespace Home.Devices.SolarEdge {

    // API documentation: https://knowledge-center.solaredge.com/sites/kc/files/se_monitoring_api.pdf

    public class SolarEdgeApiClient(string site, string apiKey) : IDisposable {

        private const string BaseUri = "https://monitoringapi.solaredge.com/";
        private const string TimeUnit = "QUARTER_OF_AN_HOUR";

        private readonly HttpClient _http = new() {
            BaseAddress = new Uri(BaseUri)
        };
        
        public async Task<SiteDetailsModel> GetSiteDetails() {
            var json = await _http.GetStringAsync($"site/{site}/details.json?api_key={apiKey}");
            var resp = JsonConvert.DeserializeObject<SiteDetailsResponseModel>(json);
            return resp.Details;
        }

        public async Task<EquipmentModel> GetInverter() {
            var json = await _http.GetStringAsync($"equipment/{site}/list.json?api_key={apiKey}");
            var list = JsonConvert.DeserializeObject<EquipmentListResponseModel>(json);
            return list.Reporters.List[0];
        }

        public async Task<DetailsModel> GetEnergy(TimeZoneInfo tz) {
            var (startTime, endTime) = GetStartEnd(tz);
            var json = await _http.GetStringAsync($"site/{site}/energyDetails.json?startTime={startTime}&endTime={endTime}&timeUnit={TimeUnit}&api_key={apiKey}");
            var details = JsonConvert.DeserializeObject<EnergyDetailsResponseModel>(json);
            return details.EnergyDetails;
        }

        public async Task<DetailsModel> GetPower(TimeZoneInfo tz) {
            var (startTime, endTime) = GetStartEnd(tz);
            var json = await _http.GetStringAsync($"site/{site}/powerDetails.json?startTime={startTime}&endTime={endTime}&timeUnit={TimeUnit}&api_key={apiKey}");
            var details = JsonConvert.DeserializeObject<PowerDetailsResponseModel>(json);
            return details.PowerDetails;
        }

        public async Task<OverviewModel> GetTotals() {
            var json = await _http.GetStringAsync($"site/{site}/overview.json?api_key={apiKey}");
            var resp = JsonConvert.DeserializeObject<OverviewResponseModel>(json);
            return resp.Overview;
        }

        private (string, string) GetStartEnd(TimeZoneInfo tz) {
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            var begin = now - TimeSpan.FromSeconds(now.Second) - TimeSpan.FromMinutes(15 + now.Minute % 15);
            var end = begin + TimeSpan.FromMinutes(15);
            return (ToDateTimeString(begin), ToDateTimeString(end));
        }

        private string ToDateTimeString(DateTime time) {
            return time.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "%20");
        }

        public void Dispose() {
            _http.Dispose();
        }

    }

}
