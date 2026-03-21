using Library.Application.DTOs;

namespace Library.Application.Interfaces;

public interface IBorrowingPatternService
{
    Task<List<BookDto>> GetAlsoBorrowed(Guid bookId);
}