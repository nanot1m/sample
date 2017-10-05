using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Vostok.Airlock;
using Vostok.Clusterclient.Topology;
using Vostok.ImageStore.Client;
using Vostok.Instrumentation.AspNetCore;
using Vostok.Logging;
using Vostok.Logging.Serilog;
using Vostok.ThumbnailGenerator.Configuration;

namespace Vostok.ThumbnailGenerator
{
    public static class EntryPoint
    {
        public static void Main()
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://+:33335")
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", false, true);
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    var airlockSection = hostingContext.Configuration.GetSection("airlock");
                    var airlockConfigSection = airlockSection.GetSection("config");
                    var airlockApiKey = airlockConfigSection.GetValue<string>("apiKey");
                    var airlockHost = airlockConfigSection.GetValue<Uri>("host");

                    var airlock = new AirlockClient(new AirlockConfig
                    {
                        ApiKey = airlockApiKey,
                        ClusterProvider = new FixedClusterProvider(airlockHost)
                    });
        
                    services.AddSingleton<IAirlockClient>(airlock);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    var serviceProvider = logging.Services.BuildServiceProvider();

                    var loggingSection = hostingContext.Configuration.GetSection("logging");

                    var rollingFileSection = loggingSection.GetSection("rollingFile");
                    var rollingFilePathFormat = rollingFileSection.GetValue<string>("pathFormat");

                    var airlockSection = loggingSection.GetSection("airlock");
                    var routingKey = airlockSection.GetValue<string>("routingKey");

                    const string outputTemplate = "{Timestamp:HH:mm:ss.fff} {Level} {Message:l} {Exception}{NewLine}{Properties}{NewLine}";

                    Log.Logger = new LoggerConfiguration()
                        .Enrich.WithHost()
                        .Enrich.WithProperty("Service", ServiceOptions.Name)
                        .WriteTo.Airlock(serviceProvider.GetService<IAirlockClient>(), routingKey)
                        .WriteTo.Async(x => x.RollingFile(rollingFilePathFormat, outputTemplate: outputTemplate))
                        .WriteTo.Console(outputTemplate: outputTemplate)
                        .CreateLogger();

                    var log = new SerilogLog(Log.Logger).WithFlowContext();

                    logging.AddVostok(log);

                    logging.Services.AddSingleton(log);
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddMvc();

                    var host = hostingContext.Configuration.GetSection("topology").GetValue<Uri>("imageStore");
                    services.AddSingleton(provider => new ImageStoreClient(provider.GetService<ILog>(), host));
                })
                .Configure(app =>
                {
                    app.UseMiddleware<RequestExecutionTimeMiddleware>();
                    app.UseDeveloperExceptionPage();
                    app.UseMvc();

                    var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
                    applicationLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
                })
                .Build()
                .Run();
        }
    }
}
