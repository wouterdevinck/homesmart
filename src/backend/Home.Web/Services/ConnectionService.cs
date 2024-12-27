using System.Threading;
using System.Threading.Tasks;
using Home.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Home.Web.Services {

    public class ConnectionService(ISmartHome home) : IHostedService {

        public async Task StartAsync(CancellationToken cancellationToken) {
            await home.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            await home.DisconnectAsync();
        }

    }

}