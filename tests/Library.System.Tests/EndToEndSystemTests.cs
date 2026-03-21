using System.Net.Http.Json;
using Library.Application.DTOs;
using Xunit;

namespace Library.Tests.System;

public class EndToEndSystemTests
{
    private readonly HttpClient _httpClient;

    public EndToEndSystemTests()
    {
        var baseUrl = Environment.GetEnvironmentVariable("BaseUrl") ?? "http://localhost:5000";
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

        var retryCount = 0;
        while (retryCount < 10)
        {
            try
            {
                var response = _httpClient.GetAsync("/api/library/most-borrowed?count=1").Result;
                if (response.IsSuccessStatusCode) break;
            }
            catch {  }

            Thread.Sleep(2000); 
            retryCount++;
        }
    }

    [Fact]
    public async Task FullFlow_UserBorrowingToPatternRecognition_ShouldSucceed()
    {
        var activityResponse = await _httpClient.GetAsync("/api/library/user-activity?startDate=2026-01-01&endDate=2026-12-31");

        // Assert
        activityResponse.EnsureSuccessStatusCode();
        var activities = await activityResponse.Content.ReadFromJsonAsync<List<BorrowerActivityDto>>();
        Assert.NotNull(activities);

        //Act
        var bookId = Guid.NewGuid();
        var patternResponse = await _httpClient.GetAsync($"/api/library/also-borrowed/{bookId}");

        // Assert
        patternResponse.EnsureSuccessStatusCode();
        var patterns = await patternResponse.Content.ReadFromJsonAsync<List<BookDto>>();
        Console.WriteLine("Tests Passed In Container");
        Assert.NotNull(patterns);
    }
}