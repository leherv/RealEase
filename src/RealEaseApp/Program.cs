using RealEaseApp;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
        webBuilder.UseKestrel().UseUrls("http://localhost:" + DeterminePort())
        .UseStartup<Startup>()
    )
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true);
        builder.AddEnvironmentVariables("RN_");
    })
    .Build()
    .RunAsync();



static string? DeterminePort()
{
    var herokuPort = Environment.GetEnvironmentVariable("PORT");
    var port = Environment.GetEnvironmentVariable("RN_CONNECTIONSTRINGS__PORT");
    return herokuPort ?? port;
}