using Grpc.Core;
using Library.Application.Interfaces;
using Library.Service;

public class LibraryGrpcService : LibraryService.LibraryServiceBase
{
    private readonly IInventoryService _inventory;
    private readonly IBorrowerActivityService _user;
    private readonly IBorrowingPatternService _pattern;
    private readonly IBorrowService _borrowService;

    public LibraryGrpcService(
        IInventoryService inventory,
        IBorrowerActivityService user,
        IBorrowingPatternService pattern,
        IBorrowService borrowService)
    {
        _inventory = inventory;
        _user = user;
        _pattern = pattern;
        _borrowService = borrowService;
    }

    public override async Task<BookList> GetMostBorrowed(
    MostBorrowedRequest request,
    ServerCallContext context)
    {
        var count = request.Count <= 0 ? 10 : request.Count;

        var data = await _inventory.GetMostBorrowed(count);

        var response = new BookList();

        response.Books.AddRange(data.Select(x => new Book
        {
            Id = x.Id.ToString(),
            Title = x.Title
        }));

        return response;
    }

    public override async Task<BorrowerActivityList> GetUserActivity(
        UserActivityRequest request,
        ServerCallContext context)
    {
        var data = await _user.GetTopBorrowerActivityAsync(
            DateTime.Parse(request.StartDate),
            DateTime.Parse(request.EndDate));

        var response = new BorrowerActivityList();

        response.Items.AddRange(data.Select(x => new BorrowerActivity
        {
            Id = x.Id.ToString(),
            Name = x.Name,
            TotalBooks = x.TotalBooks,
            ReadingPace = x.ReadingPace
        }));

        return response;
    }

    public override async Task<BookList> GetAlsoBorrowed(
        BookRequest request,
        ServerCallContext context)
    {
        var data = await _pattern.GetAlsoBorrowed(Guid.Parse(request.BookId));

        var response = new BookList();
        response.Books.AddRange(data.Select(x => new Book
        {
            Id = x.Id.ToString(),
            Title = x.Title
        }));

        return response;
    }

    public override async Task<BorrowBookResponse> BorrowBook(BorrowBookRequest request, ServerCallContext context)
    {
        var bookId = Guid.Parse(request.BookId);
        var borrowerId = Guid.Parse(request.BorrowerId);
        var borrowDate = DateTime.Parse(request.BorrowDate);

        var result = await _borrowService.BorrowBookAsync(bookId, borrowerId, borrowDate);

        return new BorrowBookResponse
        {
            Success = result.Success,
            LoanId = result.LoanId?.ToString() ?? string.Empty,
            Message = result.Message ?? string.Empty
        };
    }
}