namespace Library.Domain.Entities;

public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public int Pages { get; private set; }

    public Book(Guid id, string title, int pages)
    {
        Id = id;
        Title = title;
        Pages = pages;
    }
}