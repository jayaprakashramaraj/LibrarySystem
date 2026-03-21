using MongoDB.Bson;
using MongoDB.Driver;

namespace Library.Infrastructure.Mongo;

public static class MongoInitializer
{
    public static async Task InitializeAsync(IMongoDatabase db)
    {
        var collections = await db.ListCollectionNames().ToListAsync();

        if (!collections.Contains("patterns"))
        {
            await db.CreateCollectionAsync("patterns");
        }

        var collection = db.GetCollection<BsonDocument>("patterns");

        var indexKeys = Builders<BsonDocument>.IndexKeys.Ascending("bookId");
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(indexKeys));
    }
}