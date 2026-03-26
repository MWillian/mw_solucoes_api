using MwSolucoes.Api.Filters;
using MwSolucoes.Infrastructure;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateBootstrapLogger();

builder.Host.UseSerilog();

try
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddInfrastructure(builder.Configuration, connectionString!);

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddMvc(options => options.Filters.Add<ExceptionFilter>());

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

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