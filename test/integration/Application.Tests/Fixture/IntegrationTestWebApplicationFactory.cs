using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ReleaseNotifierApp;

namespace Application.Test.Fixture;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Startup>
{
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
            .UseStartup<Startup>();
    }
    
}