using Domain.Model;
using Infrastructure.DB.Conventions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB;

public class DatabaseContext : DbContext
{
    public DbSet<Media?> MediaDbSet { get; private set; } = null!;
    public IQueryable<Media?> Media => MediaDbSet.AsQueryable();
    
    public DbSet<Subscriber> SubscriberDbSet { get; private set; } = null!;
    public IQueryable<Subscriber> Subscribers => SubscriberDbSet.AsQueryable();
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var mediaEntity = modelBuilder.Entity<Media>();
        mediaEntity.Property(media => media.Name);
        mediaEntity
            .HasIndex(media => media.Name)
            .IsUnique();

        var subscriberEntity = modelBuilder.Entity<Subscriber>();
        subscriberEntity.Property(subscriber => subscriber.ExternalIdentifier);
        subscriberEntity.Ignore(subscriber => subscriber.SubscribedToMedia);
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
            .HasOne(subscription => subscription.Media)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        
        modelBuilder
            .UseConvention<DisableKeyValueGenerationConvention>()
            .UseConvention<EntityNamingConvention>();
       
    }
}