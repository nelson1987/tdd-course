using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class UserAuthentication
{
    public static IServiceCollection AddUserAuthentication(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        var key = Encoding.ASCII.GetBytes(Settings.Secret);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.SameSite = SameSiteMode.Strict;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        services.AddAuthorization();
        return services;
    }

    public static WebApplication UseUserAuthentication(this WebApplication app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}