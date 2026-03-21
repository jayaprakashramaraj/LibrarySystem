using Library.Application.Interfaces;
using Library.Application.Services;
using Library.Domain.Entities;
using Moq;

namespace Library.Application.Services.Tests;

public class BorrowerActivityServiceTests
{
    private readonly Mock<ILoanRepository> _loanRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IBorrowerRepository> _borrowerRepoMock;
    private readonly BorrowerActivityService _service;

    public BorrowerActivityServiceTests()
    {
        _loanRepoMock = new Mock<ILoanRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _borrowerRepoMock = new Mock<IBorrowerRepository>();

        _service = new BorrowerActivityService(
            _loanRepoMock.Object,
            _bookRepoMock.Object,
            _borrowerRepoMock.Object);
    }

    [Fact]
    public async Task GetTopBorrowerActivityAsync_ShouldReturnCharlieAndBob_ForFullDateRange()
    {
        // Arrange
        var start = new DateTime(2026, 02, 23);
        var end = new DateTime(2026, 03, 20);

        var charlieId = Guid.Parse("069DA8CF-5077-48B7-9F72-89714AE96DA8");
        var bobId = Guid.Parse("59E546AC-6F93-4E02-9BAB-59144ED4AD3E");

        var pragmaticId = Guid.Parse("5B584DB7-D7E3-43F8-BBD5-05F6F828C8F3");
        var designPatternsId = Guid.Parse("058FB3B7-2E1A-4ED9-A88E-75D2C656B181");
        var cleanCodeId = Guid.Parse("7D233186-D00C-42A0-BBE7-EB58DD6E8E8B");
        var refactoringId = Guid.Parse("FCB6CF3C-B4BB-48E8-8C39-3E8FE3DBEB4D");
        var microservicesId = Guid.Parse("9CDF9781-FECF-4AEA-8CE6-D9C54650954B");
        var systemDesignId = Guid.Parse("F7BEC58C-E33C-4757-9846-55F84B251FF1");
        var csharpInDepthId = Guid.Parse("16030561-4E65-414F-BCD5-3ACF12C9220D");

        var charlieLoans = new List<Loan>
        {
            CreateReturnedLoan(Guid.NewGuid(), refactoringId, charlieId, "2026-02-24", "2026-02-25"),
            CreateReturnedLoan(Guid.NewGuid(), designPatternsId, charlieId, "2026-02-26", "2026-03-03"),
            CreateReturnedLoan(Guid.NewGuid(), cleanCodeId, charlieId, "2026-02-26", "2026-03-03"),
            CreateReturnedLoan(Guid.NewGuid(), microservicesId, charlieId, "2026-03-02", "2026-03-03"),
            CreateReturnedLoan(Guid.NewGuid(), pragmaticId, charlieId, "2026-03-13", "2026-03-20")
        };

        var bobLoans = new List<Loan>
        {
            CreateReturnedLoan(Guid.NewGuid(), pragmaticId, bobId, "2026-02-23", "2026-02-25"),
            CreateReturnedLoan(Guid.NewGuid(), systemDesignId, bobId, "2026-02-25", "2026-02-26"),
            CreateReturnedLoan(Guid.NewGuid(), csharpInDepthId, bobId, "2026-02-28", "2026-03-06"),
            CreateReturnedLoan(Guid.NewGuid(), designPatternsId, bobId, "2026-02-28", "2026-03-05"),
            CreateReturnedLoan(Guid.NewGuid(), refactoringId, bobId, "2026-03-16", "2026-03-20")
        };

        var loans = charlieLoans.Concat(bobLoans).ToList();

        var borrowers = new Dictionary<Guid, Borrower>
        {
            { charlieId, new Borrower(charlieId, "Charlie") },
            { bobId, new Borrower(bobId, "Bob") }
        };

        var books = new Dictionary<Guid, Book>
        {
            { pragmaticId, new Book(pragmaticId, "The Pragmatic Programmer", 400) },
            { designPatternsId, new Book(designPatternsId, "Design Patterns", 395) },
            { cleanCodeId, new Book(cleanCodeId, "Clean Code", 450) },
            { refactoringId, new Book(refactoringId, "Refactoring", 500) },
            { microservicesId, new Book(microservicesId, "Microservices Patterns", 480) },
            { systemDesignId, new Book(systemDesignId, "System Design Interview", 300) },
            { csharpInDepthId, new Book(csharpInDepthId, "C# in Depth", 420) }
        };

        _loanRepoMock.Setup(r => r.GetLoansBetweenAsync(start, end)).ReturnsAsync(loans);
        _borrowerRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(borrowers);
        _bookRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(books);

        // Act
        var result = await _service.GetTopBorrowerActivityAsync(start, end);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal("Charlie", result[0].Name);
        Assert.Equal(5, result[0].TotalBooks);
        Assert.Equal(241.23, result[0].ReadingPace);

        Assert.Equal("Bob", result[1].Name);
        Assert.Equal(5, result[1].TotalBooks);
        Assert.Equal(154.8, result[1].ReadingPace);
    }

    [Fact]
    public async Task GetTopBorrowerActivityAsync_ShouldReturnFourUsers_ForShortWindow()
    {
        // Arrange
        var start = DateTime.Parse("2026-02-23");
        var end = DateTime.Parse("2026-02-24");

        var bobId = Guid.Parse("59E546AC-6F93-4E02-9BAB-59144ED4AD3E");
        var davidId = Guid.Parse("6AEDADB8-E828-41B4-A299-19D60E3D95D3");
        var charlieId = Guid.Parse("069DA8CF-5077-48B7-9F72-89714AE96DA8");
        var emmaId = Guid.Parse("BE4EC2E8-E6F2-4DF6-9E1E-6D3E863F55D7");

        var loans = new List<Loan>
        {
            CreateReturnedLoan(Guid.NewGuid(), Guid.NewGuid(), bobId, "2026-02-23", "2026-02-25"),
            CreateReturnedLoan(Guid.NewGuid(), Guid.NewGuid(), davidId, "2026-02-23", "2026-02-27"),
            CreateReturnedLoan(Guid.NewGuid(), Guid.NewGuid(), charlieId, "2026-02-24", "2026-02-25"),
            CreateReturnedLoan(Guid.NewGuid(), Guid.NewGuid(), emmaId, "2026-02-24", "2026-02-26")
        };

        _loanRepoMock.Setup(r => r.GetLoansBetweenAsync(start, end)).ReturnsAsync(loans);
        _borrowerRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new Dictionary<Guid, Borrower> {
            { bobId, new Borrower(bobId, "Bob") }, { davidId, new Borrower(davidId, "David") },
            { charlieId, new Borrower(charlieId, "Charlie") }, { emmaId, new Borrower(emmaId, "Emma") }
        });
        _bookRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new Dictionary<Guid, Book> {
            { loans[0].BookId, new Book(loans[0].BookId, "Book 1", 400) }, { loans[1].BookId, new Book(loans[1].BookId, "Book 2", 550) },
            { loans[2].BookId, new Book(loans[2].BookId, "Book 3", 500) }, { loans[3].BookId, new Book(loans[3].BookId, "Book 4", 480) }
        });

        // Act
        var result = await _service.GetTopBorrowerActivityAsync(start, end);

        // Assert
        Assert.Equal(4, result.Count);
        Assert.All(result, r => Assert.Equal(1, r.TotalBooks));
    }

    [Fact]
    public async Task GetTopBorrowerActivityAsync_ShouldHandleNullReturnDate()
    {
        // Arrange
        var start = DateTime.Parse("2026-03-11");
        var end = DateTime.Parse("2026-03-16");
        var aliceId = Guid.Parse("D27267CD-254E-4B65-ADCD-D3E6195EECA3");
        var bookId = Guid.Parse("058FB3B7-2E1A-4ED9-A88E-75D2C656B181");

        var loans = new List<Loan>
        {
            new Loan(Guid.NewGuid(), bookId, aliceId, DateTime.Parse("2026-03-11"))
        };

        _loanRepoMock.Setup(r => r.GetLoansBetweenAsync(start, end)).ReturnsAsync(loans);
        _borrowerRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new Dictionary<Guid, Borrower> { { aliceId, new Borrower(aliceId, "Alice") } });
        _bookRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new Dictionary<Guid, Book> { { bookId, new Book(bookId, "Design Patterns", 395) } });

        // Act
        var result = await _service.GetTopBorrowerActivityAsync(start, end);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result[0].TotalBooks);
        Assert.Equal(0, result[0].ReadingPace);
    }

    private Loan CreateReturnedLoan(Guid id, Guid bookId, Guid borrowerId, string borrowDate, string returnDate)
    {
        var loan = new Loan(id, bookId, borrowerId, DateTime.Parse(borrowDate));
        loan.ReturnBook(DateTime.Parse(returnDate));
        return loan;
    }
}