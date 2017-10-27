using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vostok.Instrumentation.AspNetCore;
using Vostok.Sample.VotingService.Storage;

namespace Vostok.Sample.VotingService
{
    public static class EntryPoint
    {
        public static void Main()
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://+:33336")
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
                    var connectionString = hostingContext.Configuration.GetConnectionString("CandidatesDatabase");
                    if (string.IsNullOrEmpty(connectionString))
                        services.AddSingleton<ICandidatesRepository>(new InMemoryCandidatesRepository());
                    else
                        services.AddSingleton<ICandidatesRepository>(new CandidatesRepository(new CandidatesContext(connectionString)));
                })
                .Build()
                .Run();
        }
    }
}
