using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vostok.Instrumentation.AspNetCore;
using Vostok.Logging;
using Vostok.Sample.ImageStore.Client;

namespace Vostok.Sample.ImageFilter
{
    public static class EntryPoint
    {
        public static void Main()
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://+:33337")
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", false, true);
                })
                .UseVostok()
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddMvc();
                })
                .Configure(app =>
                {
                    app.UseVostok();
                    if (app.ApplicationServices.GetRequiredService<IHostingEnvironment>().IsDevelopment())
                        app.UseDeveloperExceptionPage();
                    app.UseMvc();
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    var host = hostingContext.Configuration.GetSection("topology").GetValue<Uri>("imageStore");
                    services.AddSingleton(provider => new ImageStoreClient(provider.GetService<ILog>(), host));
                })
                .Build()
                .Run();
        }
    }
}
