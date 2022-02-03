using Home.Core.Logging;
using Home.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => {
        builder.UseStartup<Startup>();
    })
    .ConfigureLogging((hostingContext, logging) => {
        logging.ClearProviders();
        logging.AddProvider(new LoggerProvider());
    })
    .Build()
    .Run();