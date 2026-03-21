using Library.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testcontainers.MsSql;
using Testcontainers.MongoDb;

namespace Library.Tests.System;

public class FullSystemTestFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder().Build();

    public async Task InitializeAsync()
    {
        // Start real databases in Docker
        await _msSqlContainer.StartAsync();
        await _mongoContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var sqlDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<LibraryDbContext>));
            if (sqlDescriptor != null) services.Remove(sqlDescriptor);

            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(_msSqlContainer.GetConnectionString()));

            var mongoDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMongoDatabase));
            if (mongoDescriptor != null) services.Remove(mongoDescriptor);

            var mongoClient = new MongoClient(_mongoContainer.GetConnectionString());
            services.AddSingleton(mongoClient.GetDatabase("LibrarySystemTest"));

        });
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
        await _mongoContainer.StopAsync();
    }
}