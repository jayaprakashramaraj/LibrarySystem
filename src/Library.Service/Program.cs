using Library.Application.Interfaces;
using Library.Application.Services;
using Library.Infrastructure.Persistence;
using Library.Infrastructure.Seed;
using Library.Infrastructure.Mongo;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Library.Service.Interceptors;
using Library.Infrastructure.Repositories;
using Library.Application.Commands;

var builder = WebApplication.CreateBuilder(args);

// =========================================
// 1. CONFIGURATION
// =========================================
var sqlConnection = builder.Configuration.GetConnectionString("Sql");
var mongoConnection = builder.Configuration.GetConnectionString("Mongo");

Console.WriteLine($"SQL Connection: {sqlConnection}");
Console.WriteLine($"Mongo Connection: {mongoConnection}");

// =========================================
// 2. SQL SERVER (EF CORE)
// =========================================
builder.Services.AddDbContext<LibraryDbContext>(opts =>
    opts.UseSqlServer(sqlConnection, sqlOpts =>
        sqlOpts.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

// =========================================
// 3. MONGO DB
// =========================================
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(mongoConnection));

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("LibraryDb");
});

// =========================================
// 4. APPLICATION SERVICES (DIP)
// =========================================
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBorrowerRepository, BorrowerRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IPatternRepository, PatternRepository>();


builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IBorrowerActivityService, BorrowerActivityService>();
builder.Services.AddScoped<IBorrowingPatternService, BorrowingPatternService>();
builder.Services.AddScoped<IBorrowService, BorrowService>();

// MediatR (CQRS)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<BorrowBookCommand>());

// =========================================
// 5. gRPC
// =========================================
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GlobalExceptionInterceptor>();
});

// =========================================
// BUILD APP
// =========================================
var app = builder.Build();

// =========================================
// 6. DATABASE INITIALIZATION 
// =========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        Console.WriteLine("Starting database initialization...");

        var db = services.GetRequiredService<LibraryDbContext>();

        //  STEP 1: APPLY MIGRATIONS (CREATE TABLES)
        Console.WriteLine("Applying EF Core migrations...");
        await db.Database.MigrateAsync();
        Console.WriteLine(" Migrations applied successfully");

        //  STEP 2: SEED SQL DATA
        Console.WriteLine("Seeding SQL data...");
        SQLDbSeeder.Seed(db);
        Console.WriteLine("SQL seeding completed");

        //  STEP 3: INITIALIZE MONGO
        var mongoDb = services.GetRequiredService<IMongoDatabase>();

        Console.WriteLine("Initializing Mongo collections...");
        await MongoInitializer.InitializeAsync(mongoDb);
        Console.WriteLine(" Mongo collections ready");

        //  STEP 4: SEED MONGO DATA
        Console.WriteLine("Seeding Mongo data...");
        await MongoSeeder.SeedAsync(mongoDb, db);
        Console.WriteLine(" Mongo seeding completed");

        Console.WriteLine("Database initialization completed successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Database initialization failed:");
        Console.WriteLine(ex.ToString());

        throw;
    }
}

// =========================================
// 7. MAP gRPC SERVICES
// =========================================
app.MapGrpcService<LibraryGrpcService>();

app.MapGet("/", () => "Library gRPC Service is running");

// =========================================
// RUN APPLICATION
// =========================================
app.Run();