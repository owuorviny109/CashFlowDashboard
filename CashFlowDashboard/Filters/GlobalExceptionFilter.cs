using CashFlowDashboard.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CashFlowDashboard.Filters;

// Global exception filter to map service-layer exceptions to appropriate HTTP status codes.
// This follows fail-first reasoning: exceptions bubble up and are mapped to user-friendly responses.
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        // Map ValidationException to 400 Bad Request
        if (context.Exception is ValidationException validationEx)
        {
            _logger.LogWarning(validationEx, "Validation error: {Message}", validationEx.Message);
            
            context.Result = new BadRequestObjectResult(new
            {
                Error = "Validation Error",
                Message = validationEx.Message
            });
            
            context.ExceptionHandled = true;
            return;
        }

        // Map NotFoundException to 404 Not Found
        if (context.Exception is NotFoundException notFoundEx)
        {
            _logger.LogWarning(notFoundEx, "Resource not found: {Message}", notFoundEx.Message);
            
            context.Result = new NotFoundObjectResult(new
            {
                Error = "Resource Not Found",
                Message = notFoundEx.Message
            });
            
            context.ExceptionHandled = true;
            return;
        }

        // All other exceptions are logged as errors and result in 500
        _logger.LogError(context.Exception, "Unhandled exception occurred");
        
        // Let the default exception handler middleware deal with it (UseExceptionHandler)
        // This ensures we don't expose sensitive error details in production
    }
}
