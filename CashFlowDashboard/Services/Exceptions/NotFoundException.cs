namespace CashFlowDashboard.Services.Exceptions;

// Thrown when a requested entity does not exist in the data store.
// Designed to bubble up to controller layer for 404 Not Found responses.
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityType, Guid id) 
        : base($"{entityType} with ID {id} was not found.")
    {
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
