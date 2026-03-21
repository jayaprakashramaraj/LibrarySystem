using Library.Infrastructure.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Seed;

public static class MongoSeeder
{
    public static async Task SeedAsync(IMongoDatabase db, LibraryDbContext sqlDb)
    {
        var collection = db.GetCollection<BsonDocument>("patterns");

        if (await collection.Find(FilterDefinition<BsonDocument>.Empty).AnyAsync())
            return;

        var books = await sqlDb.Books.ToListAsync();

        var cleanCode = books.FirstOrDefault(x => x.Title == "Clean Code");
        var ddd = books.FirstOrDefault(x => x.Title == "Domain-Driven Design");

        if (cleanCode is null || ddd is null)
            return;

        var docs = new List<BsonDocument>
    {
        new BsonDocument {
            { "bookId", cleanCode.Id.ToString() },
            { "relatedBookId", ddd.Id.ToString() },
            { "title", ddd.Title }
        }
    };

        await collection.InsertManyAsync(docs);
    }
}