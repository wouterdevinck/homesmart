using System;
using System.Net;
using System.Threading.Tasks;
using Home.Core.Remote;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace Home.Remote.Functions {

    public class RestApi {

        [Function("RestApi")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/{*path:regex(^(?!negotiate.*$).*)}")] HttpRequestData req, string path) {
            var hub = Environment.GetEnvironmentVariable("IoTHubConnectionString");
            var device = Environment.GetEnvironmentVariable("IoTHubDeviceId");
            var remoteApiReq = new RemoteApiRequest {
                Path = req.Query.Count > 0 ? $"{path}?{req.Query}" : path,
                Method = req.Method,
                Body = req.Body.ToString()
            };
            var json = JsonConvert.SerializeObject(remoteApiReq);
            var serviceClient = ServiceClient.CreateFromConnectionString(hub);
            var commandInvocation = new CloudToDeviceMethod("api") {
                ResponseTimeout = TimeSpan.FromSeconds(30)
            };
            commandInvocation.SetPayloadJson(json);
            try {
                var result = await serviceClient.InvokeDeviceMethodAsync(device, commandInvocation);
                var payload = result.GetPayloadAsJson();
                var remoteApiResp = JsonConvert.DeserializeObject<RemoteApiResponse>(payload);
                var resp = req.CreateResponse((HttpStatusCode)remoteApiResp.Status);
                resp.Headers.Add("Content-Type", "application/json");
                await resp.WriteStringAsync(remoteApiResp.Body);
                return resp;
            } catch (Exception) {
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

    }

}