using System.IO;
using Application.Ports.Notification;
using Application.Ports.Scraper;
using FakeItEasy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReleaseNotifierApp;

namespace Application.Test.Fixture;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Startup>
{
    public readonly IScraper IntegrationTestScraper = A.Fake<IScraper>();
    public readonly IMediaNameScraper IntegrationTestMediaNameScraper = A.Fake<IMediaNameScraper>();
    public readonly INotificationService IntegrationTestNotificationService = A.Fake<INotificationService>();

    protected override IWebHostBuilder CreateWebHostBuilder() => WebHost
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration((context, builder) => builder 
            .SetBasePath(context.HostingEnvironment.ContentRootPath)
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false, reloadOnChange: true)
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.{context.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: true)
            .AddEnvironmentVariables("RN_")
        );

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseStartup<Startup>()
            .ConfigureTestServices(services =>
            {
                services.AddScoped(provider => IntegrationTestScraper);
                services.AddScoped(provider => IntegrationTestNotificationService);
                services.AddScoped(provider => IntegrationTestMediaNameScraper);
            });
    }
    
}