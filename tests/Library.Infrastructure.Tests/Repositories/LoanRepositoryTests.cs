using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories.Tests;

public class LoanRepositoryTests
{
    [Fact]
    public async Task GetLoansBetweenAsync_ShouldReturnLoans_WithinDateRange()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new LibraryDbContext(options);
        var start = new DateTime(2026, 02, 23, 0, 0, 0);
        var end = new DateTime(2026, 02, 24, 23, 59, 59);

        var loanInside1 = new Loan(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 02, 23, 10, 0, 0));
        var loanInside2 = new Loan(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 02, 24, 23, 59, 59));
        var loanOutside = new Loan(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 02, 25, 0, 0, 1));

        context.Loans.AddRange(loanInside1, loanInside2, loanOutside);
        await context.SaveChangesAsync();

        var repository = new LoanRepository(context);

        // Act
        var result = await repository.GetLoansBetweenAsync(start, end);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, l => l.Id == loanInside1.Id);
        Assert.Contains(result, l => l.Id == loanInside2.Id);
        Assert.DoesNotContain(result, l => l.Id == loanOutside.Id);
    }

    [Fact]
    public async Task GetLoansBetweenAsync_ShouldReturnEmptyList_WhenNoLoansInRange()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new LibraryDbContext(options);
        var start = new DateTime(2026, 01, 01);
        var end = new DateTime(2026, 01, 31, 23, 59, 59);

        context.Loans.Add(new Loan(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 02, 01)));
        await context.SaveChangesAsync();

        var repository = new LoanRepository(context);

        // Act
        var result = await repository.GetLoansBetweenAsync(start, end);

        // Assert
        Assert.Empty(result);
    }
}