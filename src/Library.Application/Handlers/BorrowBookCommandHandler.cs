using MediatR;
using Library.Application.Commands;
using Library.Application.Interfaces;
using Library.Domain.Entities;

namespace Library.Application.Handlers;

public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, BorrowResult>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IBorrowerRepository _borrowerRepository;

    public BorrowBookCommandHandler(ILoanRepository loanRepository, IBookRepository bookRepository, IBorrowerRepository borrowerRepository)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _borrowerRepository = borrowerRepository;
    }

    public async Task<BorrowResult> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetByIdsAsync(new List<Guid> { request.BookId });
        if (!books.TryGetValue(request.BookId, out var book))
            return new BorrowResult(false, null, "Book not found");

        var borrowers = await _borrowerRepository.GetByIdsAsync(new List<Guid> { request.BorrowerId });
        if (!borrowers.TryGetValue(request.BorrowerId, out var borrower))
            return new BorrowResult(false, null, "Borrower not found");

        var loan = new Loan(Guid.NewGuid(), request.BookId, request.BorrowerId, request.BorrowDate);

        await _loanRepository.AddAsync(loan);

        return new BorrowResult(true, loan.Id, "Borrow successful");
    }
}
