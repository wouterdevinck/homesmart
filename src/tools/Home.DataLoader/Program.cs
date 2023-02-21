using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Newtonsoft.Json;
using static Home.DataLoader.Models;

const string InfluxDbUrl = "http://192.168.1.180:8086";
const string InfluxDbToken = "xxx";
const string InfluxDbOrganization = "homesmart";
const string InfluxDbBucket = "homesmart";

const string SolarEdgeBaseUri = "https://monitoringapi.solaredge.com/";
const string SolarEdgeTimeUnit = "QUARTER_OF_AN_HOUR";
const string SolarEdgeSite = "xxx";
const string SolarEdgeApiKey = "xxx";

const string DeviceName = "solar";
const string EnergyMeasurement = "PreviousQuarterEnergy";
const string PowerMeasurement = "PreviousQuarterPower";

// TODO Automate pagination
//const string StartTime = "2023-01-01" + "%20" + "00:00:00";
//const string EndTime =   "2023-01-31" + "%20" + "23:59:59";
const string StartTime = "2023-02-01" + "%20" + "00:00:00";
const string EndTime = "2023-02-21" + "%20" + "23:59:59";

var http = new HttpClient {
    BaseAddress = new Uri(SolarEdgeBaseUri)
};

await GetData<EnergyDetailsResponseModel>("energyDetails", EnergyMeasurement, x => x.EnergyDetails.Meters[0].Values);
await GetData<PowerDetailsResponseModel>("powerDetails", PowerMeasurement, x => x.PowerDetails.Meters[0].Values);

async Task GetData<T>(string api, string measurement, Func<T, List<ValueModel>> values) {

    var url = $"site/{SolarEdgeSite}/{api}.json?startTime={StartTime}&endTime={EndTime}&timeUnit={SolarEdgeTimeUnit}&api_key={SolarEdgeApiKey}";
    var json = await http.GetStringAsync(url);
    var details = JsonConvert.DeserializeObject<T>(json);

    using var client = new InfluxDBClient(InfluxDbUrl, InfluxDbToken);
    using var writeApi = client.GetWriteApi();

    foreach (var value in values(details)) {
        var point = PointData
            .Measurement(measurement)
            .Tag("device", DeviceName)
            .Field("value", value.Value)
            .Timestamp(value.Date.ToUniversalTime(), WritePrecision.Ms);
        writeApi.WritePoint(point, InfluxDbBucket, InfluxDbOrganization);
    }

}