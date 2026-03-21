using Library.Infrastructure.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using Mongo2Go;
using Xunit;

namespace Library.Integration.Tests;

public class PatternRepositoryIntegrationTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly PatternRepository _repository;
    private const string DatabaseName = "LibraryTestDb";

    public PatternRepositoryIntegrationTests()
    {
        // Arrange
        _runner = MongoDbRunner.Start();
        _client = new MongoClient(_runner.ConnectionString);
        _database = _client.GetDatabase(DatabaseName);
        _repository = new PatternRepository(_database);
    }

    [Fact]
    public async Task GetAlsoBorrowedAsync_ShouldRetrieveAndMapData_FromInMemoryMongo()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var relatedId = Guid.NewGuid();
        var collection = _database.GetCollection<BsonDocument>("patterns");

        var doc = new BsonDocument
        {
            { "bookId", bookId.ToString() },
            { "relatedBookId", relatedId.ToString() },
            { "title", "Domain-Driven Design" }
        };

        await collection.InsertOneAsync(doc);

        // Act
        var result = await _repository.GetAlsoBorrowedAsync(bookId);

        // Assert
        Assert.Single(result);
        Assert.Equal(relatedId, result[0].RelatedBookId);
        Assert.Equal("Domain-Driven Design", result[0].Title);
    }

    [Fact]
    public async Task GetAlsoBorrowedAsync_ShouldReturnEmpty_WhenNoMatchFound()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var differentBookId = Guid.NewGuid();
        var collection = _database.GetCollection<BsonDocument>("patterns");

        await collection.InsertOneAsync(new BsonDocument
        {
            { "bookId", differentBookId.ToString() },
            { "relatedBookId", Guid.NewGuid().ToString() },
            { "title", "Clean Architecture" }
        });

        // Act
        var result = await _repository.GetAlsoBorrowedAsync(bookId);

        // Assert
        Assert.Empty(result);
    }

    public void Dispose()
    {
        // Act
        _runner.Dispose();
    }
}