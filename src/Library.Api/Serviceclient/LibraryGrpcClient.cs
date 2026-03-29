using Library.Service;

public interface ILibraryGrpcClient
{
    Task<BookList> GetMostBorrowedAsync(int count);
    Task<BorrowerActivityList> GetUserActivityAsync(DateTime start, DateTime end);
    Task<BookList> GetAlsoBorrowedAsync(Guid bookId);
    Task<BorrowBookResponse> BorrowBookAsync(Guid bookId, Guid borrowerId, DateTime borrowDate);
}

public class LibraryGrpcClientWrapper : ILibraryGrpcClient
{
    private readonly LibraryService.LibraryServiceClient _client;

    public LibraryGrpcClientWrapper(LibraryService.LibraryServiceClient client)
    {
        _client = client;
    }

    public async Task<BookList> GetMostBorrowedAsync(int count)
    {
        return await _client.GetMostBorrowedAsync(new MostBorrowedRequest { Count = count });
    }

    public async Task<BorrowerActivityList> GetUserActivityAsync(DateTime start, DateTime end)
    {
        return await _client.GetUserActivityAsync(new UserActivityRequest
        {
            StartDate = start.ToString("O"),
            EndDate = end.ToString("O")
        });
    }

    public async Task<BookList> GetAlsoBorrowedAsync(Guid bookId)
    {
        return await _client.GetAlsoBorrowedAsync(new BookRequest
        {
            BookId = bookId.ToString()
        });
    }

    public async Task<BorrowBookResponse> BorrowBookAsync(Guid bookId, Guid borrowerId, DateTime borrowDate)
    {
        return await _client.BorrowBookAsync(new BorrowBookRequest
        {
            BookId = bookId.ToString(),
            BorrowerId = borrowerId.ToString(),
            BorrowDate = borrowDate.ToString("O")
        });
    }
}