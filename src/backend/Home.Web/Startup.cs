using System;
using System.Collections.Generic;
using Home.Automations;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Interfaces;
using Home.Core.Logging;
using Home.Devices.Logo;
using Home.Devices.Zigbee;
using Home.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Home.Web {

    public class Startup {

        private static readonly string _configFilePath = Environment.GetEnvironmentVariable("HOME_CONFIG") ?? "config.yaml";

        private readonly ConfigurationReader _config = new(_configFilePath, new List<ProviderDescription> {
            LogoDeviceProvider.Descriptor, 
            ZigbeeDeviceProvider.Descriptor,
            PushOnOffAutomation.Descriptor
        });

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers().AddNewtonsoftJson();
            services.AddSingleton(_config);
            services.AddSingleton<IDeviceProvider, DeviceProviderCollection>();
            services.AddHostedService<ConnectionService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseRequestLogger();
            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }

    }

}