using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Copy the request body so it can be read multiple times
        context.Request.EnableBuffering();
        var requestBodyStream = new MemoryStream();
        await context.Request.Body.CopyToAsync(requestBodyStream);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        string requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        context.Request.Body = requestBodyStream;

        // Call the next middleware in the pipeline
        await _next(context);

        // Log the request body if the response status code is not 200
        if (context.Response.StatusCode != StatusCodes.Status200OK)
        {
            _logger.LogWarning($"Request Body: {requestBodyText}");
        }
    }
}
