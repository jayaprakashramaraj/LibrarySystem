using Library.Application.DTOs;

namespace Library.Application.Interfaces;

public interface IBorrowService
{
    Task<BorrowResultDto> BorrowBookAsync(Guid bookId, Guid borrowerId, DateTime borrowDate);
}
