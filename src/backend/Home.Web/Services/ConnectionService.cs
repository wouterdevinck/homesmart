using System.Threading;
using System.Threading.Tasks;
using Home.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Home.Web.Services {

    public class ConnectionService : IHostedService {

        private readonly ISmartHome _home;

        public ConnectionService(ISmartHome home) {
            _home = home;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            await _home.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            await _home.DisconnectAsync();
        }

    }

}