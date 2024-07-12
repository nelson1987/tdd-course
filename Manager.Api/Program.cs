using Manager.Api.Features;
using Microsoft.Extensions.Primitives;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRepositories();
builder.Services.AddUserAuthentication();
builder.Services.AddAntiforgery();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHsts();
app.UseHttpsRedirection();
app.UseUserAuthentication();
app.MapControllers();
//app.Use(async (HttpContext context, Func<Task> next) =>
//{
//    app.Logger.LogInformation("Executing the next Middleware...\r\n");
//    await next();
//    app.Logger.LogInformation("After the next Middleware call.\r\n");
//});
app.UseMiddleware<RequestContextLoggingMiddleware>(app.Logger);
app.Run();

public class RequestContextLoggingMiddleware
{
    private const string _correlationIdHeaderName = "X-Correlation-Id";
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public RequestContextLoggingMiddleware(ILogger logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId = GetCorrelationId(context);
        _logger.LogInformation("Executing the next Middleware...\r\n");

        //using (LogContext.PushProperty("CorrelationId", correlationId))
        //{
        await _next(context);
        //}
        _logger.LogInformation("After the next Middleware call.\r\n");
    }

    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            _correlationIdHeaderName, out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}

public partial class Program
{ }