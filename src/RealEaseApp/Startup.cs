﻿using System.Security.Claims;
using Application.EventHandlers;
using Application.EventHandlers.Base;
using Application.Ports.Authorization;
using Application.Ports.General;
using Application.Ports.Notification;
using Application.Ports.Persistence.Read;
using Application.Ports.Persistence.Write;
using Application.Ports.Scraper;
using Application.UseCases.Base;
using Application.UseCases.Media.AddMedia;
using Application.UseCases.Media.AddScrapeTarget;
using Application.UseCases.Media.DeleteMedia;
using Application.UseCases.Media.DeleteScrapeTarget;
using Application.UseCases.Media.QueryAvailableMedia;
using Application.UseCases.Media.QueryMedia;
using Application.UseCases.Media.QueryScrapeTargets;
using Application.UseCases.Scrape;
using Application.UseCases.Subscriber.QueryMediaSubscriptions;
using Application.UseCases.Subscriber.SubscribeMedia;
using Application.UseCases.Subscriber.UnsubscribeMedia;
using Application.UseCases.Website.QueryAvailableWebsites;
using Application.UseCases.Website.SetInactive;
using AspNet.Security.OAuth.Discord;
using AspNetCoreHero.ToastNotification;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Domain.Results;
using Infrastructure.DB;
using Infrastructure.DB.Adapters;
using Infrastructure.DB.Adapters.Repositories.Read;
using Infrastructure.DB.Adapters.Repositories.Write;
using Infrastructure.DB.DomainEvent;
using Infrastructure.Discord;
using Infrastructure.Discord.Adapters;
using Infrastructure.Discord.Settings;
using Infrastructure.General.Adapters;
using Infrastructure.Scraper.Adapters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RealEaseApp.Extensions;

namespace RealEaseApp;

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
        services.Configure<AdminSettings>(Configuration.GetSection(nameof(AdminSettings)),
            binderOptions => binderOptions.BindNonPublicProperties = true);

        // General
        services.AddControllers();
        services.AddRazorPages()
            .AddRazorPagesOptions(options => options.Conventions.AuthorizePage("/Websites", "admin"));
        services.AddHttpContextAccessor();

        // Authentication
        var discordSettings = Configuration.GetSection(nameof(DiscordSettings)).Get<DiscordSettings>();
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.AccessDeniedPath = "/AccessDenied";
            })
            .AddDiscord(options =>
            {
                options.ClientId = discordSettings.ClientId;
                options.ClientSecret = discordSettings.ClientSecret;
                options.SaveTokens = true;

                options.Events.OnCreatingTicket = async ctx =>
                {
                    // TODO: improve this
                    var client = new DiscordRestClient();
                    await client.LoginAsync(TokenType.Bearer, ctx.AccessToken);
                    var currentUserId = client.CurrentUser.Id;

                    var botAddedCondition = ctx.HttpContext.RequestServices.GetRequiredService<BotAddedCondition>();
                    var hasAddedBot = await botAddedCondition.Evaluate(currentUserId);

                    ctx.Identity.AddClaim(new Claim("botAdded", hasAddedBot.ToString()));


                    var adminCondition = ctx.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
                    var isAdmin = adminCondition.IsAdmin(currentUserId.ToString());

                    ctx.Identity.AddClaim(new Claim("isAdmin", isAdmin.ToString()));
                };
            });

        services.AddAuthorization(options =>
            options.AddPolicy("admin", policy => policy.RequireClaim("isAdmin", "True", "true")));

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
            .AddScoped<IQueryHandler<MediaQuery, Result<MediaDetails>>, QueryMediaHandler>()
            .AddScoped<IQueryHandler<AvailableWebsitesQuery, AvailableWebsites>, QueryAvailableWebsitesHandler>()
            .AddScoped<IQueryHandler<MediaSubscriptionsQuery, MediaSubscriptions>, QueryMediaSubscriptionsHandler>()
            .AddScoped<IQueryHandler<ScrapeTargetsQuery, Result<ScrapeTargets>>, QueryScrapeTargetsHandler>();

        // Commands
        services
            .AddScoped<ICommandHandler<SubscribeMediaCommand, Result>, SubscribeMediaHandler>()
            .AddScoped<ICommandHandler<UnsubscribeMediaCommand, Result>, UnsubscribeMediaHandler>()
            .AddScoped<ICommandHandler<ScrapeNewReleasesCommand, Result>, ScrapeNewReleasesHandler>()
            .AddScoped<ICommandHandler<AddMediaCommand, Result>, AddMediaHandler>()
            .AddScoped<ICommandHandler<AddScrapeTargetCommand, Result>, AddScrapeTargetHandler>()
            .AddScoped<ICommandHandler<DeleteScrapeTargetCommand, Result>, DeleteScrapeTargetHandler>()
            .AddScoped<ICommandHandler<DeleteMediaCommand, Result>, DeleteMediaHandler>()
            .AddScoped<ICommandHandler<SetWebsiteInactiveCommand, Result>, SetWebsiteInactiveHandler>();

        // Repositories(Write)
        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IMediaRepository, MediaRepository>()
            .AddScoped<ISubscriberRepository, SubscriberRepository>()
            .AddScoped<IWebsiteRepository, WebsiteRepository>();

        // Repositories(Read)
        services
            .AddScoped<IAvailableMediaReadRepository, AvailableMediaReadRepository>()
            .AddScoped<IMediaSubscriptionsReadRepository, MediaSubscriptionsReadRepository>()
            .AddScoped<IMediaReadRepository, MediaReadRepository>()
            .AddScoped<IWebsiteReadRepository, WebsiteReadRepository>()
            .AddScoped<IScrapeTargetReadRepository, ScrapeTargetReadRepository>();

        // Scraper
        services
            .AddScoped<IScraper, PlaywrightScraper>()
            .AddScoped<IMediaNameScraper, PlaywrightMediaNameScraper>();

        // General
        services
            .AddTransient<ITimeProvider, TimeProvider>()
            .AddTransient<IApplicationLogger, ApplicationLogger>()
            .AddScoped<IAuthorizationService, DiscordAuthorizationService>()
            .AddScoped<BotAddedCondition>();

        // DomainEvent
        services
            .AddScoped<IDomainEventPublisher, DomainEventPublisher>()
            .AddScoped<IDomainEventHandler, NewReleasePublishedEventHandler>();

        services.AddDbContext<DatabaseContext>(options =>
            options.UseNpgsql(GetDbConnectionString(),
                o => { o.MigrationsAssembly(typeof(DatabaseAssemblyMarker).Assembly.GetName().Name); }));

        // Jobs
        services.AddQuartzJobs(Configuration);

        // Toasts
        services.AddToastify(config =>
        {
            config.DurationInSeconds = 10;
            config.Position = Position.Right;
            config.Gravity = Gravity.Bottom;
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Lax
        });
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoint =>
        {
            endpoint.MapControllers();
            endpoint.MapRazorPages();
        });

        using var scope = app.ApplicationServices.CreateScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        databaseContext.Database.Migrate();
    }

    private string GetDbConnectionString()
    {
        var herokuConnectionString = GetHerokuConnectionString();
        return herokuConnectionString ?? Configuration.GetConnectionString("Default");
    }

    private static string? GetHerokuConnectionString()
    {
        var connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (connectionUrl == null)
            return null;

        var databaseUri = new Uri(connectionUrl);
        var db = databaseUri.LocalPath.TrimStart('/');
        var userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);
        var connectionString =
            $"Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Username={userInfo[0]};Password={userInfo[1]}";

        return connectionString;
    }
}