using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Vostok.Airlock;
using Vostok.Clusterclient.Topology;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.ImageStore.Client;
using Vostok.Instrumentation.AspNetCore;
using Vostok.Logging;
using Vostok.Logging.Serilog;
using Vostok.Metrics;
using Vostok.ThumbnailGenerator.Configuration;
using Vostok.Tracing;

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
                    var service = hostingContext.Configuration.GetValue<string>("service");
                    var project = hostingContext.Configuration.GetValue<string>("project");
                    var environment = hostingContext.Configuration.GetValue<string>("environment");
                    var routingKeyPrefix = RoutingKey.CreatePrefix(project, environment, service);
                    var airlockApiKey = airlockConfigSection.GetValue<string>("apiKey");
                    var airlockHost = airlockConfigSection.GetValue<Uri>("host");

                    var loggingSection = hostingContext.Configuration.GetSection("logging");
                    var rollingFileSection = loggingSection.GetSection("rollingFile");
                    var rollingFilePathFormat = rollingFileSection.GetValue<string>("pathFormat");
                    var loggerConfiguration = new LoggerConfiguration()
                        .WriteTo.Async(x => x.RollingFile(rollingFilePathFormat))
                        .CreateLogger();
                    var airlockLog = new SerilogLog(loggerConfiguration);
                    var airlockClient = new AirlockClient(new AirlockConfig
                    {
                        ApiKey = airlockApiKey,
                        ClusterProvider = new FixedClusterProvider(airlockHost)
                    }, airlockLog.WithFlowContext());
                    services.AddSingleton<IAirlockClient>(airlockClient);
                    var metricConfiguration = GetMetricConfiguration(airlockClient, routingKeyPrefix, environment);
                    new RootMetricScope(metricConfiguration).SystemMetrics(10.Seconds());
                    services.AddSingleton<IMetricConfiguration>(metricConfiguration);
                    InitializeTracing(routingKeyPrefix, airlockClient);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    var serviceProvider = logging.Services.BuildServiceProvider();

                    var loggingSection = hostingContext.Configuration.GetSection("logging");

                    var rollingFileSection = loggingSection.GetSection("rollingFile");
                    var rollingFilePathFormat = rollingFileSection.GetValue<string>("pathFormat");

                    var service = hostingContext.Configuration.GetValue<string>("service");
                    var project = hostingContext.Configuration.GetValue<string>("project");
                    var environment = hostingContext.Configuration.GetValue<string>("environment");
                    var routingKeyPrefix = RoutingKey.Create(project, environment, service, "logs");

                    const string outputTemplate = "{Timestamp:HH:mm:ss.fff} {Level} {Message:l} {Exception}{NewLine}{Properties}{NewLine}";

                    Log.Logger = new LoggerConfiguration()
                        .Enrich.WithHost()
                        .Enrich.WithProperty("Service", service)
                        .WriteTo.Airlock(serviceProvider.GetService<IAirlockClient>(), routingKeyPrefix)
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
                    app.UseMiddleware<RequestExecutionDistributedContextMiddleware>();
                    app.UseMiddleware<RequestExecutionTraceMiddleware>(ServiceOptions.Name);
                    app.UseDeveloperExceptionPage();
                    app.UseMvc();

                    var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
                    applicationLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
                })
                .Build()
                .Run();
        }

        private static MetricConfiguration GetMetricConfiguration(AirlockClient airlockClient, string routingKeyPrefix,
            string environment)
        {
            var airlockMetricReporter = new AirlockMetricReporter(airlockClient, routingKeyPrefix);
            var metricConfiguration = new MetricConfiguration
            {
                Reporter = airlockMetricReporter,
                Environment = environment
            };
            return metricConfiguration;
        }

        private static void InitializeTracing(string routingKeyPrefix, AirlockClient airlockClient)
        {
            var tracingRoutingKey = RoutingKey.AddSuffix(routingKeyPrefix, "traces");
            Trace.Configuration.AirlockRoutingKey = () => tracingRoutingKey;
            Trace.Configuration.AirlockClient = airlockClient;
            Trace.Configuration.IsEnabled = () => true;
        }
    }
}
