using Library.Api.Middleware;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Library.Api.MiddleWares.Tests
{
    public class GlobalExceptionMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_NoException_CallsNext()
        {
            // Arrange
            var context = new DefaultHttpContext();

            context.Response.Body = new MemoryStream();

            var middleware = new GlobalExceptionMiddleware(async (ctx) =>
            {
                await ctx.Response.WriteAsync("OK");
            });

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(context.Response.Body).ReadToEndAsync();

            Assert.Equal("OK", response);
        }

        [Fact]
        public async Task InvokeAsync_ArgumentException_Returns400()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var middleware = new GlobalExceptionMiddleware((ctx) =>
            {
                throw new ArgumentException("Invalid input");
            });

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var json = JsonDocument.Parse(body);

            Assert.Equal(400, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);
            Assert.Equal("Invalid input", json.RootElement.GetProperty("error").GetString());
        }

        [Fact]
        public async Task InvokeAsync_Exception_Returns500()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var middleware = new GlobalExceptionMiddleware((ctx) =>
            {
                throw new Exception("Some failure");
            });

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var json = JsonDocument.Parse(body);

            Assert.Equal(500, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);
            Assert.Equal("Internal server error", json.RootElement.GetProperty("error").GetString());
        }
    }
}