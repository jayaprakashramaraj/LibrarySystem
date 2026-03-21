using Library.Domain.Entities;
using Library.Infrastructure.Persistence;

namespace Library.Infrastructure.Seed;

public static class SQLDbSeeder
{
    public static void Seed(LibraryDbContext db)
    {
        if (db.Books.Any()) return; 

        // Books
        var books = new List<Book>
        {
            new(Guid.NewGuid(), "Clean Code", 450),
            new(Guid.NewGuid(), "Domain-Driven Design", 550),
            new(Guid.NewGuid(), "The Pragmatic Programmer", 400),
            new(Guid.NewGuid(), "Refactoring", 500),
            new(Guid.NewGuid(), "Design Patterns", 395),
            new(Guid.NewGuid(), "Microservices Patterns", 480),
            new(Guid.NewGuid(), "System Design Interview", 300),
            new(Guid.NewGuid(), "C# in Depth", 420)
        };

        db.Books.AddRange(books);

        // Borrowers
        var borrowers = new List<Borrower>
        {
            new(Guid.NewGuid(), "Alice"),
            new(Guid.NewGuid(), "Bob"),
            new(Guid.NewGuid(), "Charlie"),
            new(Guid.NewGuid(), "David"),
            new(Guid.NewGuid(), "Emma")
        };

        db.Borrowers.AddRange(borrowers);

        var random = new Random();

        // Loans 
        var loans = new List<Loan>();

        foreach (var borrower in borrowers)
        {
            foreach (var book in books.OrderBy(_ => random.Next()).Take(random.Next(2, 6)))
            {
                var borrowDate = DateTime.UtcNow.AddDays(-random.Next(1, 30));
                var returnDate = borrowDate.AddDays(random.Next(1, 10));

                var loan = new Loan(Guid.NewGuid(), book.Id, borrower.Id, borrowDate);
                loan.ReturnBook(returnDate);

                loans.Add(loan);
            }
        }

        db.Loans.AddRange(loans);

        db.SaveChanges();
    }
}