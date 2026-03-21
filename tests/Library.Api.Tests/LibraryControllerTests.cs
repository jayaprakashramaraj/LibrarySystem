using Grpc.Core;
using Library.Api.Controllers;
using Library.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Library.Api.Controllers.Tests
{

    public class LibraryControllerTests
    {
        private readonly Mock<ILibraryGrpcClient> _mockClient;
        private readonly LibraryController _controller;

        public LibraryControllerTests()
        {
            _mockClient = new Mock<ILibraryGrpcClient>();
            _controller = new LibraryController(_mockClient.Object);
        }

        // =========================
        // MOST BORROWED
        // =========================

        [Fact]
        public async Task GetMostBorrowed_ReturnsOk_WithData()
        {
            // Arrange
            var response = new BookList();
            response.Books.Add(new Book { Id = "1", Title = "Clean Code" });

            _mockClient.Setup(x => x.GetMostBorrowedAsync(5))
                       .ReturnsAsync(response);

            // Act
            var result = await _controller.GetMostBorrowed(5);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<Book>>(ok.Value);

            Assert.Single(data);
        }

        [Fact]
        public async Task GetMostBorrowed_ReturnsOk_EmptyList()
        {
            // Arrange
            var response = new BookList();

            _mockClient.Setup(x => x.GetMostBorrowedAsync(5))
                       .ReturnsAsync(response);

            // Act
            var result = await _controller.GetMostBorrowed(5);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<Book>>(ok.Value);

            Assert.Empty(data);
        }

        [Fact]
        public async Task GetMostBorrowed_WhenGrpcThrows_Returns500()
        {
            // Arrange
            _mockClient.Setup(x => x.GetMostBorrowedAsync(It.IsAny<int>()))
                .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "grpc error")));

            // Act
            var result = await _controller.GetMostBorrowed(5);

            // Assert
            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
        }

        // =========================
        // USER ACTIVITY
        // =========================

        [Fact]
        public async Task GetUserActivity_ReturnsOk_WithData()
        {
            // Arrange
            var response = new BorrowerActivityList();
            response.Items.Add(new BorrowerActivity
            {
                Id = "1",
                Name = "Alice",
                TotalBooks = 3,
                ReadingPace = 25.5
            });

            _mockClient.Setup(x => x.GetUserActivityAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                       .ReturnsAsync(response);

            // Act
            var result = await _controller.GetUserActivity(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<BorrowerActivity>>(ok.Value);

            Assert.Single(data);
        }

        [Fact]
        public async Task GetUserActivity_ReturnsOk_Empty()
        {
            // Arrange
            var response = new BorrowerActivityList();

            _mockClient.Setup(x => x.GetUserActivityAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                       .ReturnsAsync(response);

            // Act
            var result = await _controller.GetUserActivity(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<BorrowerActivity>>(ok.Value);

            Assert.Empty(data);
        }

        [Fact]
        public async Task GetUserActivity_WhenGrpcFails_Returns500()
        {
            // Arrange
            _mockClient.Setup(x => x.GetUserActivityAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "error")));

            // Act
            var result = await _controller.GetUserActivity(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow);

            // Assert
            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
        }

        // =========================
        // ALSO BORROWED
        // =========================

        [Fact]
        public async Task GetAlsoBorrowed_ReturnsOk_WithData()
        {
            // Arrange
            var response = new BookList();
            response.Books.Add(new Book { Id = "2", Title = "DDD" });

            _mockClient.Setup(x => x.GetAlsoBorrowedAsync(It.IsAny<Guid>()))
                       .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAlsoBorrowed(Guid.NewGuid());

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<Book>>(ok.Value);

            Assert.Single(data);
        }

        [Fact]
        public async Task GetAlsoBorrowed_ReturnsOk_Empty()
        {
            // Arrange
            var response = new BookList();

            _mockClient.Setup(x => x.GetAlsoBorrowedAsync(It.IsAny<Guid>()))
                       .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAlsoBorrowed(Guid.NewGuid());

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<Book>>(ok.Value);

            Assert.Empty(data);
        }

        [Fact]
        public async Task GetAlsoBorrowed_WhenGrpcFails_Returns500()
        {
            // Arrange
            _mockClient.Setup(x => x.GetAlsoBorrowedAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "error")));

            // Act
            var result = await _controller.GetAlsoBorrowed(Guid.NewGuid());

            // Assert
            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
        }

        [Fact]
        public void HandleGrpcException_ReturnsProperResponse()
        {
            // Arrange
            var ex = new RpcException(new Status(StatusCode.Internal, "test error"));

            // Act
            var result = _controller
                .GetType()
                .GetMethod("HandleGrpcException", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(_controller, new object[] { ex });

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }
    }
}