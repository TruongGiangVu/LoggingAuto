using Castle.DynamicProxy;
using LoggingAuto.Interceptors;
using LoggingAuto.Middlewares;
using LoggingAuto.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File($"logs/.log",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true,
        retainedFileCountLimit: null,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
    // .WriteTo.RollingFile($"logs/{DateTime.Today.ToString("dd-MM-yyyy")}.log", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
    .CreateLogger();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInterceptedSingleton<IUserService, UserService, LoggingInterceptor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<LoggingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

