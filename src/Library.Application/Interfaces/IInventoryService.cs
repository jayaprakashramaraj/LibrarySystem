using Library.Application.DTOs;

namespace Library.Application.Interfaces;

public interface IInventoryService
{
    Task<List<BookDto>> GetMostBorrowed(int count);
}