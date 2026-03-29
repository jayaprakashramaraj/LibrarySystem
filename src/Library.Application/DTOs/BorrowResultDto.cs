namespace Library.Application.DTOs;

public record BorrowResultDto(bool Success, Guid? LoanId, string? Message);
