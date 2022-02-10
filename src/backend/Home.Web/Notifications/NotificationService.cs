using System;
using System.Threading;
using System.Threading.Tasks;
using Home.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace Home.Web.Notifications {

    public class NotificationService : IHostedService {

        private readonly IHubContext<NotificationHub> _hub;
        private readonly IDeviceProvider _deviceProvider;

        public NotificationService(IHubContext<NotificationHub> hub, IDeviceProvider deviceProvider) {
            _hub = hub;
            _deviceProvider = deviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            Action<IDevice> subscribe = device => {
                device.DeviceUpdate += async (_, _) => {
                    await _hub.Clients.All.SendAsync("deviceupdates", device);
                };
            };
            foreach (var device in _deviceProvider.GetDevices()) {
                subscribe(device);
            }
            _deviceProvider.DeviceDiscovered += (_, device) => {
                subscribe(device);
            };
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

    }

}