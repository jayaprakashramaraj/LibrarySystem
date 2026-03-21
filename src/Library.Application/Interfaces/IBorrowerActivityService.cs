using Library.Application.DTOs;

namespace Library.Application.Interfaces;

public interface IBorrowerActivityService
{
    Task<List<BorrowerActivityDto>> GetTopBorrowerActivityAsync(DateTime start, DateTime end);
}