using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Library.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.Infrastructure.Repositories.Tests;



public class BorrowerRepositoryTests
{
    [Fact]
    public async Task GetByIdsAsync_ShouldReturnDictionary_WhenIdsMatch()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new LibraryDbContext(options);
        var aliceId = Guid.Parse("D27267CD-254E-4B65-ADCD-D3E6195EECA3");
        var charlieId = Guid.Parse("069DA8CF-5077-48B7-9F72-89714AE96DA8");

        context.Borrowers.AddRange(
            new Borrower(aliceId, "Alice"),
            new Borrower(charlieId, "Charlie")
        );
        await context.SaveChangesAsync();

        var repository = new BorrowerRepository(context);

        // Act
        var result = await repository.GetByIdsAsync(new List<Guid> { aliceId, charlieId });

        // Assert
        Assert.Equal(2, result.Count);
        Assert.True(result.ContainsKey(aliceId));
        Assert.Equal("Alice", result[aliceId].Name);
        Assert.True(result.ContainsKey(charlieId));
        Assert.Equal("Charlie", result[charlieId].Name);
    }

    [Fact]
    public async Task GetByIdsAsync_ShouldReturnEmptyDictionary_WhenNoIdsMatch()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new LibraryDbContext(options);
        var repository = new BorrowerRepository(context);

        // Act
        var result = await repository.GetByIdsAsync(new List<Guid> { Guid.NewGuid() });

        // Assert
        Assert.Empty(result);
    }
}