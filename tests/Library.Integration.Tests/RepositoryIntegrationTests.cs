using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Library.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Library.Integration.Tests;


public class RepositoryIntegrationTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly LibraryDbContext _context;

    public RepositoryIntegrationTests()
    {
        // Arrange
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new LibraryDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetMostBorrowedAsync_ShouldReturnActualDatabaseResults()
    {
        // Arrange
        var book = new Book(Guid.NewGuid(), "Domain-Driven Design", 500);
        var borrowerId = Guid.NewGuid();
        var loan1 = new Loan(Guid.NewGuid(), book.Id, borrowerId, DateTime.Now);
        var loan2 = new Loan(Guid.NewGuid(), book.Id, borrowerId, DateTime.Now);

        _context.Books.Add(book);
        _context.Loans.AddRange(loan1, loan2);
        await _context.SaveChangesAsync();

        var repository = new BookRepository(_context);

        // Act
        var result = await repository.GetMostBorrowedAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Domain-Driven Design", result[0].Book.Title);
        Assert.Equal(2, result[0].BorrowCount);
    }

    [Fact]
    public async Task GetLoansBetweenAsync_ShouldFilterByDateTimeCorrectly()
    {
        // Arrange
        var repository = new LoanRepository(_context);
        var start = new DateTime(2026, 01, 01);
        var end = new DateTime(2026, 01, 31, 23, 59, 59);

        var validLoan = new Loan(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 01, 15));
        var invalidLoan = new Loan(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 02, 01));

        _context.Loans.AddRange(validLoan, invalidLoan);
        await _context.SaveChangesAsync();

        // Act
        var result = await repository.GetLoansBetweenAsync(start, end);

        // Assert
        Assert.Single(result);
        Assert.Equal(validLoan.Id, result[0].Id);
    }

    public void Dispose()
    {
        // Assert
        _context.Dispose();
        _connection.Close();
    }
}