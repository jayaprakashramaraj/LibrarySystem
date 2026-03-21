using Library.Domain.Entities;

namespace Library.Application.Interfaces
{
    public interface IBookRepository
    {
        Task<List<(Book Book, int BorrowCount)>> GetMostBorrowedAsync(int count);
        Task<Dictionary<Guid, Book>> GetByIdsAsync(List<Guid> ids);
    }
}
