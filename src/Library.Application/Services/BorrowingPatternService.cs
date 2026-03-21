using Library.Application.DTOs;
using Library.Application.Interfaces;

namespace Library.Application.Services;

public class BorrowingPatternService : IBorrowingPatternService
{
    private readonly IPatternRepository _repository;

    public BorrowingPatternService(IPatternRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BookDto>> GetAlsoBorrowed(Guid bookId)
    {
        var data = await _repository.GetAlsoBorrowedAsync(bookId);

        return data
            .Select(x => new BookDto(
                x.RelatedBookId,
                x.Title
            ))
            .ToList();
    }
}