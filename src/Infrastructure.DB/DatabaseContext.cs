using Domain.Model;
using Infrastructure.DB.Conventions;
using Infrastructure.DB.DomainEvent;
using Infrastructure.DB.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB;

public class DatabaseContext : DbContext
{
    public DbSet<Media> MediaDbSet { get; private set; } = null!;
    public IQueryable<Media> Media => MediaDbSet.AsQueryable();
    
    public DbSet<Subscriber> SubscriberDbSet { get; private set; } = null!;
    public IQueryable<Subscriber> Subscribers => SubscriberDbSet.AsQueryable();
    
    public DbSet<Subscription> SubscriptionDbSet { get; private set; } = null!;
    public IQueryable<Subscription> Subscriptions => SubscriptionDbSet.AsQueryable();
    
    public DbSet<Website> WebsiteDbSet { get; private set; } = null!;
    public IQueryable<Website> Websites => WebsiteDbSet.AsQueryable();
    
    private readonly IDomainEventPublisher _domainEventPublisher;
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options, IDomainEventPublisher domainEventPublisher)
        : base(options)
    {
        _domainEventPublisher = domainEventPublisher;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var websiteEntity = modelBuilder.Entity<Website>();
        websiteEntity.Property(website => website.Name);
        websiteEntity.Property(website => website.Url);
        
        var scrapeTargetEntity = modelBuilder.Entity<ScrapeTarget>();
        scrapeTargetEntity.Property(scrapeTarget => scrapeTarget.RelativeUrl);
        scrapeTargetEntity
            .HasOne<Website>()
            .WithMany()
            .HasForeignKey(scrapeTarget => scrapeTarget.WebsiteId);
        
        var mediaEntity = modelBuilder.Entity<Media>();
        mediaEntity.Property(media => media.Name);
        mediaEntity.OwnsOne(media => media.NewestRelease, releaseEntity =>
        {
            releaseEntity.Property(release => release.Link);
            releaseEntity.OwnsOne(release => release.ReleaseNumber, releaseNumberEntity =>
            {
                releaseNumberEntity.Property(releaseNumber => releaseNumber.Major);
                releaseNumberEntity.Property(releaseNumber => releaseNumber.Minor);
            });
        });
        mediaEntity.HasOne(media => media.ScrapeTarget);
        mediaEntity
            .HasIndex(media => media.Name)
            .IsUnique();

        var subscriberEntity = modelBuilder.Entity<Subscriber>();
        subscriberEntity.Property(subscriber => subscriber.ExternalIdentifier);
        subscriberEntity.Ignore(subscriber => subscriber.SubscribedToMediaIds);
        subscriberEntity
            .HasIndex(subscriber => subscriber.ExternalIdentifier)
            .IsUnique();
        subscriberEntity
            .HasMany(subscriber => subscriber.Subscriptions)
            .WithOne()
            .HasForeignKey("SubscriberId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        var subscriptionEntity = modelBuilder.Entity<Subscription>();
        subscriptionEntity.Property(subscription => subscription.MediaId);
        subscriptionEntity.Property(subscription => subscription.SubscriberId);
        subscriptionEntity.HasIndex(subscription => new { subscription.SubscriberId, subscription.MediaId })
            .IsUnique();
        subscriptionEntity
            .HasOne<Media>()
            .WithMany()
            .HasForeignKey(media => media.MediaId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .UseConvention<DisableKeyValueGenerationConvention>()
            .UseConvention<EntityNamingConvention>();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Publish the domain events immediately before saving the changes. This means that all further changes triggered by the domain events happen in the same transaction by default.
        await _domainEventPublisher.PublishDomainEvents(this);
        return await base.SaveChangesAsync(cancellationToken);
    }
}