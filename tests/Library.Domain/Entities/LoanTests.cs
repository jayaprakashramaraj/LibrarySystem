using Library.Domain.Entities;
using Xunit;

namespace Library.Domain.Tests;

public class LoanTests
{
    [Fact]
    public void CalculateReadingPace_ShouldReturnZero_WhenNotReturned()
    {
        // Arrange
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Parse("2026-02-23"));

        // Act
        var pace = loan.CalculateReadingPace(400);

        // Assert
        Assert.Equal(0, pace);
    }

    [Fact]
    public void CalculateReadingPace_ShouldUseMinimumOneDay_WhenReturnedSameDay()
    {
        // Arrange
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Parse("2026-02-23 10:00"));
        loan.ReturnBook(DateTime.Parse("2026-02-23 14:00"));

        // Act
        var pace = loan.CalculateReadingPace(300);

        // Assert
        Assert.Equal(300, pace);
    }

    [Theory]
    [InlineData("2026-02-23", "2026-02-25", 400, 200)] 
    [InlineData("2026-02-23", "2026-02-27", 550, 137.5)] 
    [InlineData("2026-03-13", "2026-03-20", 400, 57.14)] 
    public void CalculateReadingPace_ShouldCalculateCorrectly_ForVariousDurations(string borrow, string @return, int pages, double expectedPace)
    {
        // Arrange
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Parse(borrow));
        loan.ReturnBook(DateTime.Parse(@return));

        // Act
        var pace = loan.CalculateReadingPace(pages);

        // Assert
        Assert.Equal(expectedPace, pace, 2);
    }
}