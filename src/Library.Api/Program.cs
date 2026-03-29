using Library.Api.Middleware;
using Library.Service;

var builder = WebApplication.CreateBuilder(args);

// =========================================
// CONFIGURATION
// =========================================
var grpcUrl = Environment.GetEnvironmentVariable("GRPC__LibraryService")
              ?? builder.Configuration["GRPC:LibraryService"]
             ?? Environment.GetEnvironmentVariable("LIBRARY_SERVICE_ADDR");

// If not provided, allow a sensible local default when running in Development
if (string.IsNullOrWhiteSpace(grpcUrl))
{
    if (builder.Environment.IsDevelopment())
    {
        // Local development default (use the typical Kestrel port used by Library.Service when running locally)
        grpcUrl = "http://localhost:6001";
    }
    else
    {
        throw new Exception("gRPC service URL is not configured. Set 'GRPC:LibraryService' in configuration or environment variable 'GRPC__LibraryService' or 'LIBRARY_SERVICE_ADDR'.");
    }
}

// =========================================    
// gRPC CLIENT 
// =========================================
builder.Services.AddGrpcClient<LibraryService.LibraryServiceClient>(options =>
{
    options.Address = new Uri(grpcUrl);
});

builder.Services.AddScoped<ILibraryGrpcClient, LibraryGrpcClientWrapper>();

// =========================================
// CONTROLLERS (REST API)
// =========================================
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =========================================
// BUILD APP
// =========================================
var app = builder.Build();

// =========================================
// MIDDLEWARE PIPELINE
// =========================================
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.MapGet("/", () => "Library API Gateway is running");

// =========================================
// RUN
// =========================================
app.Run();