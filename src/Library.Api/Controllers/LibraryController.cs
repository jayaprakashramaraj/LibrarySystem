using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/library")]
public class LibraryController : ControllerBase
{
    private readonly ILibraryGrpcClient _client;

    public LibraryController(ILibraryGrpcClient client)
    {
        _client = client;
    }

    [HttpGet("most-borrowed")]
    public async Task<IActionResult> GetMostBorrowed([FromQuery] int count = 10)
    {
        try
        {
            var response = await _client.GetMostBorrowedAsync(count);

            return Ok(response.Books);
        }
        catch (RpcException ex)
        {
            return HandleGrpcException(ex);
        }
    }

    [HttpGet("user-activity")]
    public async Task<IActionResult> GetUserActivity(DateTime startDate, DateTime endDate)
    {
        try
        {
            var result = await _client.GetUserActivityAsync(startDate, endDate);

            return Ok(result.Items);
        }
        catch (RpcException ex)
        {
            return HandleGrpcException(ex);
        }
    }

    [HttpGet("also-borrowed/{bookId}")]
    public async Task<IActionResult> GetAlsoBorrowed(Guid bookId)
    {
        try
        {
            var result = await _client.GetAlsoBorrowedAsync(bookId);

            return Ok(result.Books);
        }
        catch (RpcException ex)
        {
            return HandleGrpcException(ex);
        }
    }

    private IActionResult HandleGrpcException(RpcException ex)
    {
        return StatusCode(500, new { source = "Library service", error = ex.Status.Detail });
    }
}