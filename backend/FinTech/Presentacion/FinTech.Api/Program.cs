using FinTech.Api.Middleware;
using FinTech.Application.Interfaces.Repositories;
using FinTech.Application.Interfaces.Services;
using FinTech.Application.UseCases;
using FinTech.Infrastructure.Persistencia;
using FinTech.Infrastructure.Persistencia.Repositorios;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, services, config) =>
        config.ReadFrom.Configuration(ctx.Configuration)
              .ReadFrom.Services(services)
              .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
              .WriteTo.File("logs/sgip-.txt",
                  rollingInterval: RollingInterval.Day,
                  outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                  retainedFileCountLimit: 7));

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? Environment.GetEnvironmentVariable("DATABASE_URL")
        ?? throw new InvalidOperationException("Connection string no configurada.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddScoped<ILoanRepository, LoanRepository>();
    builder.Services.AddScoped<IPaymentScheduleRepository, PaymentScheduleRepository>();
    builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

    builder.Services.AddScoped<ILoanService, LoanService>();
    builder.Services.AddScoped<ITransactionService, TransactionService>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "SGIP - FinTech API",
            Version = "v1",
            Description = "Sistema de Gestion de Inversiones y Préstamos"
        });
    });

    builder.Services.AddCors(options =>
        options.AddPolicy("FrontendPolicy", policy =>
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        Log.Information("Migraciones generadas correctamente.");
    }

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseCors("FrontendPolicy");

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGIP API v1");
        c.RoutePrefix = "swagger";
    });

    app.MapControllers();

    Log.Information("SGIP API iniciada correctamente.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación no pudo iniciar.");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
