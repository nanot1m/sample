using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vostok.Instrumentation.AspNetCore;
using Vostok.Sample.ImageStore.Controllers;

namespace Vostok.Sample.ImageStore
{
    public static class EntryPoint
    {
        public static void Main()
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://+:33334")
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
                    app.UseDeveloperExceptionPage();
                    app.UseMvc();
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddSingleton<IImagesRepository>(new InMemoryImagesRepository());
                })
                .Build()
                .Run();
        }
    }
}
