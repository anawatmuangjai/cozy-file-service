using CozyFileService.WebApi;
using Serilog;
using Microsoft.Extensions.Azure;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("CozyFileService API Starting up!");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, service, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(service)
    .Enrich.FromLogContext()
    .WriteTo.Console(), 
    true);

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();

app.UseSerilogRequestLogging();

await app.SetDatabaseAsync();

app.Run();
