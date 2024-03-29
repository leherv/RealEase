﻿using Application.Ports.Persistence.Write;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Write;

public class SubscriberRepository : ISubscriberRepository
{
    private readonly DatabaseContext _databaseContext;

    public SubscriberRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<Subscriber?> GetByExternalId(string externalId)
    {
        return await _databaseContext.Subscribers
            .Include(subscriber => subscriber.Subscriptions)
            .SingleOrDefaultAsync(subscriber => subscriber.ExternalIdentifier.Equals(externalId));
    }

    public async Task AddSubscriber(Subscriber subscriber)
    {
        await _databaseContext.SubscriberDbSet.AddAsync(subscriber);
    }

    public async Task<IReadOnlyCollection<Subscriber>> GetAllSubscribersByMediaId(Guid mediaId)
    {
        return await  _databaseContext.Subscribers
            .Include(subscriber => subscriber.Subscriptions)
            .Where(subscriber => subscriber.Subscriptions.Any(subscription => subscription.MediaId == mediaId))
            .ToListAsync();
    }
}