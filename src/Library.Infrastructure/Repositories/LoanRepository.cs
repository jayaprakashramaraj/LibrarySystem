using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly LibraryDbContext _db;

    public LoanRepository(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<List<Loan>> GetLoansBetweenAsync(DateTime start, DateTime end)
    {
        return await _db.Loans
            .Where(x => x.BorrowDate >= start && x.BorrowDate <= end)
            .ToListAsync();
    }
}