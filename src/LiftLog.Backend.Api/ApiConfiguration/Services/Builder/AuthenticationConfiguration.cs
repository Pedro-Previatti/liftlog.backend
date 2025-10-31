using System.Text;
using LiftLog.Backend.Core.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LiftLog.Backend.Api.ApiConfiguration.Services.Builder;

/// <summary>
/// Provides configuration method for registering application authorization.
/// </summary>
public static class AuthenticationConfiguration
{
    /// <summary>
    /// Adds application authentication
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance used to configure services.</param>
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        var httpsMetadata = !builder.Environment.IsDevelopment();

        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
        if (jwtOptions == null || string.IsNullOrWhiteSpace(jwtOptions.Key))
            throw new InvalidOperationException(
                "Jwt:Key is required. Provide it via appsettings or env var JWT__Key."
            );

        builder
            .Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = httpsMetadata;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key)
                    ),
                    ValidateLifetime = true,
                    ClockSkew = TokenValidationParameters.DefaultClockSkew,
                };
            });
    }
}
