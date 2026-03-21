namespace Library.Domain.Entities;

public class Borrower
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public Borrower(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}