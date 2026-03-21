using Library.Infrastructure.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace Library.Infrastructure.Repositories.Tests;


public class PatternRepositoryTests
{
    private readonly Mock<IMongoCollection<BsonDocument>> _collectionMock;
    private readonly Mock<IMongoDatabase> _databaseMock;
    private readonly PatternRepository _repository;

    public PatternRepositoryTests()
    {
        _collectionMock = new Mock<IMongoCollection<BsonDocument>>();
        _databaseMock = new Mock<IMongoDatabase>();

        _databaseMock.Setup(db => db.GetCollection<BsonDocument>("patterns", null))
            .Returns(_collectionMock.Object);

        _repository = new PatternRepository(_databaseMock.Object);
    }

    [Fact]
    public async Task GetAlsoBorrowedAsync_ShouldReturnMappedList_WhenDataExists()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var relatedId1 = Guid.NewGuid();
        var relatedId2 = Guid.NewGuid();

        var docs = new List<BsonDocument>
        {
            new BsonDocument { { "relatedBookId", relatedId1.ToString() }, { "title", "Clean Code" } },
            new BsonDocument { { "relatedBookId", relatedId2.ToString() }, { "title", "Refactoring" } }
        };

        var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
        cursorMock.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        cursorMock.Setup(c => c.Current).Returns(docs);

        _collectionMock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        // Act
        var result = await _repository.GetAlsoBorrowedAsync(bookId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(relatedId1, result[0].RelatedBookId);
        Assert.Equal("Clean Code", result[0].Title);
        Assert.Equal(relatedId2, result[1].RelatedBookId);
        Assert.Equal("Refactoring", result[1].Title);
    }

    [Fact]
    public async Task GetAlsoBorrowedAsync_ShouldReturnEmptyList_WhenNoDocumentsFound()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
        cursorMock.Setup(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _collectionMock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        // Act
        var result = await _repository.GetAlsoBorrowedAsync(bookId);

        // Assert
        Assert.Empty(result);
    }
}