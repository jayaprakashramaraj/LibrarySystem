namespace Library.Application.DTOs;

public record BorrowerActivityDto(
    Guid Id,
    string Name,
    int TotalBooks,
    double ReadingPace
);