using MongoDB.Bson;
using MongoDB.Driver;
using Library.Application.Interfaces;

namespace Library.Infrastructure.Mongo;

public class PatternRepository : IPatternRepository
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public PatternRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<BsonDocument>("patterns");
    }

    public async Task<List<(Guid RelatedBookId, string Title)>> GetAlsoBorrowedAsync(Guid bookId)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("bookId", bookId.ToString());

        var result = await _collection.Find(filter).ToListAsync();

        return result
            .Select(x => (
                Guid.Parse(x["relatedBookId"].AsString),
                x["title"].AsString
            ))
            .ToList();
    }
}