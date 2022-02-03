using System.Threading;
using System.Threading.Tasks;
using Home.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Home.Web.Services {

    public class ConnectionService : IHostedService {

        private readonly IDeviceProvider _deviceProvider;

        public ConnectionService(IDeviceProvider deviceProvider) {
            _deviceProvider = deviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            await _deviceProvider.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            await _deviceProvider.DisconnectAsync();
        }

    }

}