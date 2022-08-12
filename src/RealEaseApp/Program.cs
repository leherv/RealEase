using RealEaseApp;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
        webBuilder.UseKestrel().UseUrls("http://0.0.0.0:" + Environment.GetEnvironmentVariable("PORT"))
        .UseStartup<Startup>()
        // .UseWebRoot(Environment.CurrentDirectory)
    )
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true);
        builder.AddEnvironmentVariables("RN_");
    })
    .Build()
    .RunAsync();