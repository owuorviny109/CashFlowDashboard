namespace CashFlowDashboard.Services.Exceptions;

// Thrown when business logic validation fails (e.g., invalid amount, invalid date range).
// Designed to bubble up to controller layer for 400 Bad Request responses.
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
