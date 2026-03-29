using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Domain.Entities;

namespace Library.Application.Services;

public class BorrowService : IBorrowService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IBorrowerRepository _borrowerRepository;

    public BorrowService(
        ILoanRepository loanRepository,
        IBookRepository bookRepository,
        IBorrowerRepository borrowerRepository)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _borrowerRepository = borrowerRepository;
    }

    public async Task<BorrowResultDto> BorrowBookAsync(Guid bookId, Guid borrowerId, DateTime borrowDate)
    {
        // Basic validations
        var books = await _bookRepository.GetByIdsAsync(new List<Guid> { bookId });
        if (!books.TryGetValue(bookId, out var book))
            return new BorrowResultDto(false, null, "Book not found");

        var borrowers = await _borrowerRepository.GetByIdsAsync(new List<Guid> { borrowerId });
        if (!borrowers.TryGetValue(borrowerId, out var borrower))
            return new BorrowResultDto(false, null, "Borrower not found");

        var loan = new Loan(Guid.NewGuid(), bookId, borrowerId, borrowDate);

        await _loanRepository.AddAsync(loan);

        return new BorrowResultDto(true, loan.Id, "Borrow successful");
    }
}
