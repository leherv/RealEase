using Application.EventHandlers;
using Application.EventHandlers.Base;
using Application.Ports.General;
using Application.Ports.Notification;
using Application.Ports.Persistence.Read;
using Application.Ports.Persistence.Write;
using Application.Ports.Scraper;
using Application.UseCases.Base;
using Application.UseCases.Media.AddMedia;
using Application.UseCases.Media.QueryAvailableMedia;
using Application.UseCases.Media.QueryScrapeTargets;
using Application.UseCases.Scrape;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Application.UseCases.Subscriber.SubscribeMedia;
using Application.UseCases.Subscriber.UnsubscribeMedia;
using Application.UseCases.Website.QueryAvailableWebsites;
using Discord.Commands;
using Discord.WebSocket;
using Domain.Results;
using Infrastructure.DB;
using Infrastructure.DB.Adapters;
using Infrastructure.DB.Adapters.Repositories.Read;
using Infrastructure.DB.Adapters.Repositories.Write;
using Infrastructure.DB.DomainEvent;
using Infrastructure.Discord;
using Infrastructure.Discord.Settings;
using Infrastructure.General.Adapters;
using Infrastructure.Scraper;
using Microsoft.EntityFrameworkCore;
using ReleaseNotifierApp.Extensions;

namespace ReleaseNotifierApp;

public class Startup
{
    private IConfiguration Configuration { get; }
    private IWebHostEnvironment HostEnvironment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        Configuration = configuration;
        HostEnvironment = hostingEnvironment;
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        // Settings
        services.Configure<DiscordSettings>(Configuration.GetSection(nameof(DiscordSettings)));

        // Discord
        services
            .AddHostedService<DiscordService>()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandlingService>()
            .AddScoped<INotificationService, DiscordNotificationService>();

        // Use Cases
        // Base
        services
            .AddScoped<ICommandDispatcher, CommandDispatcher>()
            .AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Queries
        services
            .AddScoped<IQueryHandler<AvailableMediaQuery, AvailableMedia>, QueryAvailableMediaHandler>()
            .AddScoped<IQueryHandler<AvailableWebsitesQuery, AvailableWebsites>, QueryAvailableWebsitesHandler>()
            .AddScoped<IQueryHandler<MediaSubscriptionsQuery, MediaSubscriptions>, QueryMediaSubscriptionsHandler>()
            .AddScoped<IQueryHandler<ScrapeTargetsQuery, Result<ScrapeTargets>>, QueryScrapeTargetsHandler>();

        // Commands
        services
            .AddScoped<ICommandHandler<SubscribeMediaCommand, Result>, SubscribeMediaHandler>()
            .AddScoped<ICommandHandler<UnsubscribeMediaCommand, Result>, UnsubscribeMediaHandler>()
            .AddScoped<ICommandHandler<ScrapeNewReleasesCommand, Result>, ScrapeNewReleasesHandler>()
            .AddScoped<ICommandHandler<AddMediaCommand, Result>, AddMediaHandler>();
        
        // Repositories(Write)
        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IMediaRepository, MediaRepository>()
            .AddScoped<ISubscriberRepository, SubscriberRepository>()
            .AddScoped<IWebsiteRepository, WebsiteRepository>();
        
        // Repositories(Read)
        services
            .AddScoped<IMediaReadRepository, MediaReadRepository>()
            .AddScoped<IMediaSubscriptionsReadRepository, MediaSubscriptionsReadRepository>()
            .AddScoped<IWebsiteReadRepository, WebsiteReadRepository>()
            .AddScoped<IScrapeTargetReadRepository, ScrapeTargetReadRepository>();
        
        // Scraper
        services
            .AddScoped<IScraper, PlaywrightScraper>()
            .AddScoped<IMediaNameScraper, PlaywrightMediaNameScraper>();
        
        // General
        services
            .AddTransient<ITimeProvider, TimeProvider>()
            .AddTransient<IApplicationLogger, ApplicationLogger>();
        
        // DomainEvent
        services
            .AddScoped<IDomainEventPublisher, DomainEventPublisher>()
            .AddScoped<IDomainEventHandler, NewReleasePublishedEventHandler>();

        services.AddDbContext<DatabaseContext>(options =>
            options.UseNpgsql(GetDbConnectionString(),
                o => { o.MigrationsAssembly(typeof(DatabaseAssemblyMarker).Assembly.GetName().Name); }));
        
        // Jobs
        services.AddQuartzJobs(Configuration);
    }
    
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        databaseContext.Database.Migrate();
    }

    private string GetDbConnectionString()
    {
        var herokuConnectionString = GetHerokuConnectionString();
        return herokuConnectionString ?? Configuration.GetConnectionString("Default");
    }
    
    private static string? GetHerokuConnectionString() {
        var connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (connectionUrl == null)
            return null;

        var databaseUri = new Uri(connectionUrl);
        var db = databaseUri.LocalPath.TrimStart('/');
        var userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);
        var connectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Username={userInfo[0]};Password={userInfo[1]}";
        
        return connectionString;
    }
}