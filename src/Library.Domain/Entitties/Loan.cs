namespace Library.Domain.Entities;

public class Loan
{
    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public Guid BorrowerId { get; private set; }

    public DateTime BorrowDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    public Loan(Guid id, Guid bookId, Guid borrowerId, DateTime borrowDate)
    {
        Id = id;
        BookId = bookId;
        BorrowerId = borrowerId;
        BorrowDate = borrowDate;
    }

    public void ReturnBook(DateTime returnDate)
    {
        ReturnDate = returnDate;
    }

    public double CalculateReadingPace(int pages)
    {
        if (!ReturnDate.HasValue) return 0;

        var days = Math.Max(1, (ReturnDate.Value - BorrowDate).Days);
        return pages / (double)days;
    }
}