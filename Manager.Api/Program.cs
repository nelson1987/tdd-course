using Manager.Api.Features;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Primitives;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddCors();
builder.Services.ConfigureHttpJsonOptions(_ => { });
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
        .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRepositories();
builder.Services.AddUserAuthentication();
builder.Services.AddResponseCaching();
builder.Services.AddAntiforgery();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<EntryMeter>();
builder.Services.AddLogging();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("CofeeShop")
                .AddAttributes(new Dictionary<string, object>
                {
                    ["environment.name"] = "production",
                    ["team.name"] = "backend"
                })
    )
    .WithMetrics(metrics =>
        metrics
        .AddMeter(EntryMeter.MeterName)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter()
        )
    .WithTracing(tracing =>
        tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter());
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.AddOtlpExporter();
    logging.AddConsoleExporter();
});

var app = builder.Build();

app.UseExceptionHandler(x =>
{
    x.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        app.Logger.LogError("Exception: {exception}", contextFeature!.Error.GetBaseException().Message);
        if (contextFeature is not null)
        {
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCodes = context.Response.StatusCode,
                Message = "Internal Server Error",
                Error = contextFeature.Error.Message
            });
        }
    });
});
/*
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature == null) return;

        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = contextFeature.Error switch
        {
            //BadRequestException => (int)HttpStatusCode.BadRequest,
            OperationCanceledException => (int)HttpStatusCode.ServiceUnavailable,
            //NotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var errorResponse = new
        {
            statusCode = context.Response.StatusCode,
            message = contextFeature.Error.GetBaseException().Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    });
});
*/
app.UseSwagger();
app.UseSwaggerUI();
app.UseHsts();
app.UseHttpsRedirection();
app.UseUserAuthentication();
app.MapControllers();
app.UseMiddleware<RequestContextLoggingMiddleware>(app.Logger);
app.UseResponseCaching();
app.Run();

//public partial class CacheDependencies
//{
//    public IServiceCollection AddRedisCache(this IServiceCollection services)
//    {
//        services.AddMemoryCache();
//        return services;
//    }
//}

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

public class EntryMeter
{
    public Meter Meter { get; private set; }
    public Counter<int> ReadsCounter { get; private set; }
    public static readonly string MeterName = "MinhaAplicacao";

    public EntryMeter()
    {
        Meter = new Meter(MeterName, "1.0.0");
        ReadsCounter = Meter.CreateCounter<int>("entry.reads", "Number of account reads");
    }
}

public partial class Program
{ }