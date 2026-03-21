using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories.Tests;


public class BookRepositoryTests
{
    [Fact]
    public async Task GetByIdsAsync_ShouldReturnDictionary_WhenIdsMatch()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: "GetByIdsDb")
            .Options;

        using var context = new LibraryDbContext(options);
        var pragmaticId = Guid.Parse("5B584DB7-D7E3-43F8-BBD5-05F6F828C8F3");
        var cleanCodeId = Guid.Parse("7D233186-D00C-42A0-BBE7-EB58DD6E8E8B");

        context.Books.AddRange(
            new Book(pragmaticId, "The Pragmatic Programmer", 400),
            new Book(cleanCodeId, "Clean Code", 450)
        );
        await context.SaveChangesAsync();

        var repository = new BookRepository(context);

        // Act
        var result = await repository.GetByIdsAsync(new List<Guid> { pragmaticId, cleanCodeId });

        // Assert
        Assert.Equal(2, result.Count);
        Assert.True(result.ContainsKey(pragmaticId));
        Assert.Equal("The Pragmatic Programmer", result[pragmaticId].Title);
    }

    [Fact]
    public async Task GetMostBorrowedAsync_ShouldReturnOrderedBooks_BasedOnLoanCount()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: "MostBorrowedDb")
            .Options;

        using var context = new LibraryDbContext(options);
        var pragmaticId = Guid.Parse("5B584DB7-D7E3-43F8-BBD5-05F6F828C8F3");
        var cleanCodeId = Guid.Parse("7D233186-D00C-42A0-BBE7-EB58DD6E8E8B");

        context.Books.AddRange(
            new Book(pragmaticId, "The Pragmatic Programmer", 400),
            new Book(cleanCodeId, "Clean Code", 450)
        );

        context.Loans.AddRange(
            new Loan(Guid.NewGuid(), pragmaticId, Guid.NewGuid(), DateTime.Now),
            new Loan(Guid.NewGuid(), pragmaticId, Guid.NewGuid(), DateTime.Now),
            new Loan(Guid.NewGuid(), cleanCodeId, Guid.NewGuid(), DateTime.Now)
        );
        await context.SaveChangesAsync();

        var repository = new BookRepository(context);

        // Act
        var result = await repository.GetMostBorrowedAsync(2);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("The Pragmatic Programmer", result[0].Book.Title);
        Assert.Equal(2, result[0].BorrowCount);
        Assert.Equal("Clean Code", result[1].Book.Title);
        Assert.Equal(1, result[1].BorrowCount);
    }
}