using Library.Application.DTOs;
using Library.Application.Interfaces;
using MediatR;
using Library.Application.Commands;

namespace Library.Application.Services;

public class BorrowService : IBorrowService
{
    private readonly IMediator _mediator;

    public BorrowService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<BorrowResultDto> BorrowBookAsync(Guid bookId, Guid borrowerId, DateTime borrowDate)
    {
        var result = await _mediator.Send(new BorrowBookCommand(bookId, borrowerId, borrowDate));

        return new BorrowResultDto(result.Success, result.LoanId, result.Message);
    }
}
