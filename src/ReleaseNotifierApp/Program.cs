using ReleaseNotifierApp;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
        webBuilder.UseKestrel().UseUrls("http://0.0.0.0:" + Environment.GetEnvironmentVariable("PORT"))
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