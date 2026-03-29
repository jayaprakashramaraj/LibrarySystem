using Library.Application.DTOs;
using Library.Application.Interfaces;

namespace Library.Application.Services;

public class BorrowerActivityService : IBorrowerActivityService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IBorrowerRepository _borrowerRepository;

    public BorrowerActivityService(
        ILoanRepository loanRepository,
        IBookRepository bookRepository,
        IBorrowerRepository borrowerRepository)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _borrowerRepository = borrowerRepository;
    }

    public async Task<List<BorrowerActivityDto>> GetTopBorrowerActivityAsync(DateTime start, DateTime end)
    {
        var loans = await _loanRepository.GetLoansBetweenAsync(start, end);

        if (loans == null || !loans.Any())
            return new List<BorrowerActivityDto>();

        var userGroups = loans.GroupBy(l => l.BorrowerId).ToList();
        int maxBooksCount = userGroups.Max(g => g.Count());

        var borrowerIds = userGroups.Select(g => g.Key).ToList();
        var bookIds = loans.Select(l => l.BookId).Distinct().ToList();

        var borrowers = await _borrowerRepository.GetByIdsAsync(borrowerIds);
        var books = await _bookRepository.GetByIdsAsync(bookIds);

        return userGroups
            .Where(g => g.Count() == maxBooksCount)
            .Select(g =>
            {
                borrowers.TryGetValue(g.Key, out var borrower);

                var returnedLoans = g.Where(l => l.ReturnDate.HasValue).ToList();

                double calculatedPace = 0;
                if (returnedLoans.Any())
                {
                    calculatedPace = returnedLoans.Average(l =>
                    {
                        if (books.TryGetValue(l.BookId, out var book))
                        {
                            var duration = (l.ReturnDate.Value - l.BorrowDate).TotalDays;
                            double days = duration < 1 ? 1 : duration;
                            return book.Pages / days;
                        }
                        return 0;
                    });
                }

                return new BorrowerActivityDto(
                    g.Key,
                    borrower?.Name ?? "Unknown",
                    maxBooksCount,
                    Math.Round(calculatedPace, 2)
                );
            })
            .OrderByDescending(x => x.ReadingPace)
            .ToList();
    }

}