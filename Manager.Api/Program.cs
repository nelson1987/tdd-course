using Manager.Api.Features;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRepositories();
builder.Services.AddUserAuthentication();
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters.NameClaimType = "sub";
//    options.MetadataAddress = "https://login.microsoftonline.com/common/.well-known/openid-configuration";
//    options.Audience = "https://myapi.audience.com";
//});
////builder.Services.AddAuthorization(
////    options =>
////    {
////        options.AddPolicy("RouteMustMatchSubject", builder => builder.Requirements.Add(
////            new SubjectMustMatchRouteParameterRequirement("sub", "customerId")));
////    });
////builder.Services.AddSingleton<IAuthorizationHandler, SubjectMustMatchRouteParameterHandler>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseUserAuthentication();
app.MapControllers();
app.Run();

public partial class Program
{ }