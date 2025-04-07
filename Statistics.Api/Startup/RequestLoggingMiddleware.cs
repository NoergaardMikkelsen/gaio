public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
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
            Console.WriteLine($"Request Body: {requestBodyText}");
        }
    }
}
