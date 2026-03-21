using Library.Api.Middleware;
using Library.Service;

var builder = WebApplication.CreateBuilder(args);

// =========================================
// CONFIGURATION
// =========================================
var grpcUrl = builder.Configuration["GRPC:LibraryService"];

if (string.IsNullOrEmpty(grpcUrl))
{
    throw new Exception("gRPC service URL is not configured.");
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
if (app.Environment.IsDevelopment())
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