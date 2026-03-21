namespace Library.Application.Interfaces;

public interface IPatternRepository
{
    Task<List<(Guid RelatedBookId, string Title)>> GetAlsoBorrowedAsync(Guid bookId);
}