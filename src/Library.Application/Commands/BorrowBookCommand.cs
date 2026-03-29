using MediatR;

namespace Library.Application.Commands;

public record BorrowBookCommand(Guid BookId, Guid BorrowerId, DateTime BorrowDate) : IRequest<BorrowResult>;

public record BorrowResult(bool Success, Guid? LoanId, string? Message);
