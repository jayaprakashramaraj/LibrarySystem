using Library.Domain.Entities;

namespace Library.Application.Interfaces;

public interface IBorrowerRepository
{
    Task<Dictionary<Guid, Borrower>> GetByIdsAsync(List<Guid> ids);
}