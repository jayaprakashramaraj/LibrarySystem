using Library.Application.Interfaces;
using Library.Domain.Entities;
using Moq;

namespace Library.Application.Services.Tests
{

    public class InventoryServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly InventoryService _service;

        public InventoryServiceTests()
        {
            _bookRepoMock = new Mock<IBookRepository>();
            _service = new InventoryService(_bookRepoMock.Object);
        }

        [Fact]
        public async Task GetMostBorrowed_ShouldReturnBookDtos_WhenDataExists()
        {
            // Arrange
            int requestedCount = 2;
            var pragmaticId = Guid.Parse("5B584DB7-D7E3-43F8-BBD5-05F6F828C8F3");
            var cleanCodeId = Guid.Parse("7D233186-D00C-42A0-BBE7-EB58DD6E8E8B");

            var mockData = new List<(Book Book, int BorrowCount)>
        {
            (new Book(pragmaticId, "The Pragmatic Programmer", 400), 10),
            (new Book(cleanCodeId, "Clean Code", 450), 8)
        };

            _bookRepoMock.Setup(r => r.GetMostBorrowedAsync(requestedCount))
                .ReturnsAsync(mockData);

            // Act
            var result = await _service.GetMostBorrowed(requestedCount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(pragmaticId, result[0].Id);
            Assert.Equal("The Pragmatic Programmer", result[0].Title);
            Assert.Equal("Clean Code", result[1].Title);
            _bookRepoMock.Verify(r => r.GetMostBorrowedAsync(requestedCount), Times.Once);
        }

        [Fact]
        public async Task GetMostBorrowed_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            int requestedCount = 5;
            _bookRepoMock.Setup(r => r.GetMostBorrowedAsync(requestedCount))
                .ReturnsAsync(new List<(Book Book, int BorrowCount)>());

            // Act
            var result = await _service.GetMostBorrowed(requestedCount);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _bookRepoMock.Verify(r => r.GetMostBorrowedAsync(requestedCount), Times.Once);
        }
    }
}