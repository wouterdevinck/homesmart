using System;
using System.Threading;
using System.Threading.Tasks;
using Home.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace Home.Web.Notifications {

    public class NotificationService : IHostedService {

        private readonly IHubContext<NotificationHub> _hub;
        private readonly ISmartHome _home;

        public NotificationService(IHubContext<NotificationHub> hub, ISmartHome home) {
            _hub = hub;
            _home = home;
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            Action<IDevice> subscribe = device => {
                device.DeviceUpdate += async (_, _) => {
                    await _hub.Clients.All.SendAsync("deviceupdates", device);
                };
            };
            foreach (var device in _home.GetDevices()) {
                subscribe(device);
            }
            _home.DeviceDiscovered += (_, device) => {
                subscribe(device);
            };
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

    }

}