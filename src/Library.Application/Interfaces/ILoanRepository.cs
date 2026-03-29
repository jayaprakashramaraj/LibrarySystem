using Library.Domain.Entities;

namespace Library.Application.Interfaces
{
    public interface ILoanRepository
    {
        Task<List<Loan>> GetLoansBetweenAsync(DateTime start, DateTime end);
        Task AddAsync(Loan loan);
    }
}
