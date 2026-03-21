using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _db;

    public BookRepository(LibraryDbContext db)
    {
        _db = db;
    }
    public async Task<Dictionary<Guid, Book>> GetByIdsAsync(List<Guid> ids)
    {
        return await _db.Books
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);
    }
    public async Task<List<(Book Book, int BorrowCount)>> GetMostBorrowedAsync(int count)
    {
        var topBooksData = await _db.Loans
            .GroupBy(l => l.BookId)
            .Select(g => new
            {
                BookId = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .Join(_db.Books,
                loanGrp => loanGrp.BookId,
                book => book.Id,
                (loanGrp, book) => new { Book = book, loanGrp.Count })
            .ToListAsync();

        return topBooksData
            .Select(x => (x.Book, x.Count))
            .ToList();
    }
}