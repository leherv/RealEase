using System.Linq;
using Application.Test.Fixture.Givens;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;

namespace Application.Test.Extensions
{
    public static class DatabaseContextExtensions
    {
        public static DatabaseContext Clear(this DatabaseContext databaseContext)
        {
            return databaseContext
                .DeleteAll<Subscriber>()
                .DeleteAll<Media>()
                .DeleteAll<Website>()
                .DeleteAll();
        }

        public static DatabaseContext Seed(this DatabaseContext databaseContext, GivenTheData givenTheData)
        {
            databaseContext
                .SeedWebsites(givenTheData)
                .SeedMedia(givenTheData)
                .SeedSubscribers(givenTheData);
               
        
            return databaseContext;
        }

        private static DatabaseContext SeedMedia(this DatabaseContext databaseContext, GivenTheData givenTheData)
        {
            databaseContext.MediaDbSet.AddRange(givenTheData.Media.PersistedMediaList);
            databaseContext.SaveChanges();
        
            return databaseContext;
        }
        
        private static DatabaseContext SeedSubscribers(this DatabaseContext databaseContext, GivenTheData givenTheData)
        {
            databaseContext.SubscriberDbSet.AddRange(givenTheData.Subscriber.Subscribers);
            databaseContext.SaveChanges();
        
            return databaseContext;
        }
        
        private static DatabaseContext SeedWebsites(this DatabaseContext databaseContext, GivenTheData givenTheData)
        {
            databaseContext.WebsiteDbSet.AddRange(givenTheData.Website.Websites);
            databaseContext.SaveChanges();
        
            return databaseContext;
        }

        private static DatabaseContext DeleteAll<T>(this DatabaseContext databaseContext)
        {
            var entityType = databaseContext.Model.FindEntityType(typeof(T));

            databaseContext.Database.ExecuteSqlRaw($"DELETE FROM {entityType.GetFullTableName()}");

            return databaseContext;
        }

        private static DatabaseContext DeleteAll(this DatabaseContext databaseContext)
        {
            var deleteStatements = databaseContext.Model.GetEntityTypes()
                .Select(type => type.GetFullTableName())
                .Where(type => type != null)
                .Select(type => type!)
                .OrderBy(tableName => tableName)
                .Select(tableName => $"DELETE FROM {tableName}");

            var joinedDbStatements = string.Join(";", deleteStatements);

            databaseContext.Database.ExecuteSqlRaw(joinedDbStatements);

            return databaseContext;
        }
    }
}