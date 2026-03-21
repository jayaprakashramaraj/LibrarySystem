using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Borrower> Borrowers => Set<Borrower>();
    public DbSet<Loan> Loans => Set<Loan>();

}
