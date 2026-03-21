using Library.Application.DTOs;
using Library.Application.Interfaces;

namespace Library.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IBookRepository _bookRepository;

    public InventoryService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<List<BookDto>> GetMostBorrowed(int count)
    {
        var result = await _bookRepository.GetMostBorrowedAsync(count);
        return result.Select(x => new BookDto(x.Book.Id, x.Book.Title)).ToList();
    }
}