using Library.Application.Interfaces;
using Moq;

namespace Library.Application.Services.Tests;

public class BorrowingPatternServiceTests
{
    private readonly Mock<IPatternRepository> _repositoryMock;
    private readonly BorrowingPatternService _service;

    public BorrowingPatternServiceTests()
    {
        _repositoryMock = new Mock<IPatternRepository>();
        _service = new BorrowingPatternService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAlsoBorrowed_ShouldReturnBookDtos_WhenDataExists()
    {
        // Arrange
        var targetBookId = Guid.NewGuid();
        var mockData = new List<dynamic>
        {
            new { RelatedBookId = Guid.NewGuid(), Title = "Clean Architecture" },
            new { RelatedBookId = Guid.NewGuid(), Title = "Domain-Driven Design" }
        }.Cast<dynamic>().ToList();

        _repositoryMock.Setup(repo => repo.GetAlsoBorrowedAsync(targetBookId))
            .ReturnsAsync(mockData.Select(d => (RelatedBookId: (Guid)d.RelatedBookId, Title: (string)d.Title)).ToList());

        // Act
        var result = await _service.GetAlsoBorrowed(targetBookId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Clean Architecture", result[0].Title);
        Assert.Equal("Domain-Driven Design", result[1].Title);
        _repositoryMock.Verify(repo => repo.GetAlsoBorrowedAsync(targetBookId), Times.Once);
    }

    [Fact]
    public async Task GetAlsoBorrowed_ShouldReturnEmptyList_WhenNoDataFound()
    {
        // Arrange
        var targetBookId = Guid.NewGuid();
        _repositoryMock.Setup(repo => repo.GetAlsoBorrowedAsync(targetBookId))
            .ReturnsAsync(new List<(Guid RelatedBookId, string Title)>());

        // Act
        var result = await _service.GetAlsoBorrowed(targetBookId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _repositoryMock.Verify(repo => repo.GetAlsoBorrowedAsync(targetBookId), Times.Once);
    }
}