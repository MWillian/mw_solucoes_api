using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using MwSolucoes.Api.Filters;
using MwSolucoes.Application;
using MwSolucoes.Domain.Enums;
using MwSolucoes.Infrastructure;

using Serilog;
using System.Net;
using System.Text;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://meusite.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        var errorResponse = new { message = "Você atingiu o limite de requisições permitidas. Tente novamente mais tarde." };
        await context.HttpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "global";

        return RateLimitPartition.GetTokenBucketLimiter(clientIp, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 60,
            QueueLimit = 0,
            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
            TokensPerPeriod = 20
        });
    });

    options.AddPolicy("auth", httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "auth_global";

        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
        {
            Window = TimeSpan.FromMinutes(1),
            PermitLimit = 5,
            QueueLimit = 0
        });
    });
});

try
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddInfrastructure(builder.Configuration, connectionString!);
    builder.Services.AddApplication();

    var signingKey = builder.Configuration.GetValue<string>("Settings:Jwt:SigningKey") ?? string.Empty;
    var issuer = builder.Configuration.GetValue<string>("Settings:Jwt:Issuer") ?? string.Empty;
    var audience = builder.Configuration.GetValue<string>("Settings:Jwt:Audience") ?? string.Empty;

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("AdminAccess", policy =>
        policy.RequireClaim("access_level", AccessLevels.Admin.ToString()))
        .AddPolicy("Technician", policy =>
        policy.RequireClaim("role", UserRoles.Técnico.ToString()));

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddMvc(options => options.Filters.Add<ExceptionFilter>());

    var app = builder.Build();

    app.UseHttpsRedirection();
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseRouting();

    app.UseCors("FrontendCorsPolicy");

    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none';");

        await next();
    });

    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação falhou ao iniciar.");
}
finally
{
    Log.CloseAndFlush();
}