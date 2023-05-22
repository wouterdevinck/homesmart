using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Home.Remote.Functions {

    public class Api {

        [Function("Negotiate")]
        public static string Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/negotiate")] HttpRequestData req, [SignalRConnectionInfoInput(HubName = "notifications")] string connectionInfo) {
            return connectionInfo;
        }

    }

}