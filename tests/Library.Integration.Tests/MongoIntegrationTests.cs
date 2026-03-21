using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Library.Integration.Tests;
public class MongoIntegrationTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly IMongoDatabase _database;

    public MongoIntegrationTests()
    {
        // Arrange
        _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        _database = client.GetDatabase("TestDb");
    }

    [Fact]
    public void Test_Mongo_Logic()
    {
        // Act
        var collection = _database.GetCollection<BsonDocument>("test");
        collection.InsertOne(new BsonDocument { { "name", "test" } });

        // Assert
        Assert.Equal(1, collection.CountDocuments(new BsonDocument()));
    }

    public void Dispose()
    {
        // Act
        _runner.Dispose();
    }
}