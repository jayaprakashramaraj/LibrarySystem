using Library.Application.DTOs;
using Library.Application.Interfaces;
using Moq;

namespace Library.Service.Tests.Grpc;

public class LibraryGrpcServiceTests
{
    private readonly Mock<IInventoryService> _inventoryMock;
    private readonly Mock<IBorrowerActivityService> _userMock;
    private readonly Mock<IBorrowingPatternService> _patternMock;
    private readonly Mock<IBorrowService> _borrowMock;
    private readonly LibraryGrpcService _service;

    public LibraryGrpcServiceTests()
    {
        _inventoryMock = new Mock<IInventoryService>();
        _userMock = new Mock<IBorrowerActivityService>();
        _patternMock = new Mock<IBorrowingPatternService>();
        _borrowMock = new Mock<IBorrowService>();

        _service = new LibraryGrpcService(
            _inventoryMock.Object,
            _userMock.Object,
            _patternMock.Object,
            _borrowMock.Object
            );
    }

    [Fact]
    public async Task GetMostBorrowed_ShouldReturnBookList()
    {
        // Arrange
        var request = new MostBorrowedRequest { Count = 5 };
        var mockData = new List<BookDto>
        {
            new BookDto(Guid.NewGuid(), "Clean Code")
        };

        _inventoryMock.Setup(s => s.GetMostBorrowed(5))
            .ReturnsAsync(mockData);

        // Act
        var result = await _service.GetMostBorrowed(request, null!);

        // Assert
        Assert.Single(result.Books);
        Assert.Equal("Clean Code", result.Books[0].Title);
        _inventoryMock.Verify(s => s.GetMostBorrowed(5), Times.Once);
    }

    [Fact]
    public async Task GetUserActivity_ShouldReturnActivityList()
    {
        // Arrange
        var request = new UserActivityRequest
        {
            StartDate = "2026-02-23",
            EndDate = "2026-03-20"
        };

        var mockData = new List<BorrowerActivityDto>
        {
            new BorrowerActivityDto(Guid.NewGuid(), "Charlie", 5, 241.23)
        };

        _userMock.Setup(s => s.GetTopBorrowerActivityAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _service.GetUserActivity(request, null!);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("Charlie", result.Items[0].Name);
        Assert.Equal(5, result.Items[0].TotalBooks);
        Assert.Equal(241.23, result.Items[0].ReadingPace);
    }

    [Fact]
    public async Task GetAlsoBorrowed_ShouldReturnRelatedBooks()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var request = new BookRequest { BookId = bookId.ToString() };
        var mockData = new List<BookDto>
        {
            new BookDto(Guid.NewGuid(), "Refactoring")
        };

        _patternMock.Setup(s => s.GetAlsoBorrowed(bookId))
            .ReturnsAsync(mockData);

        // Act
        var result = await _service.GetAlsoBorrowed(request, null!);

        // Assert
        Assert.Single(result.Books);
        Assert.Equal("Refactoring", result.Books[0].Title);
        _patternMock.Verify(s => s.GetAlsoBorrowed(bookId), Times.Once);
    }
}