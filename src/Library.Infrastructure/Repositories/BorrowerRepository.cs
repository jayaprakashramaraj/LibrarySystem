using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories
{
    public class BorrowerRepository : IBorrowerRepository
    {
        private readonly LibraryDbContext _db;

        public BorrowerRepository(LibraryDbContext db)
        {
            _db = db;
        }
        public async Task<Dictionary<Guid, Borrower>> GetByIdsAsync(List<Guid> ids)
        {
            return await _db.Borrowers
                .Where(x => ids.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);
        }
    }
}
