using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using POSHWeb.Client.Web.Core.Extensions;
using POSHWeb.Data;
using POSHWeb.Options;
using POSHWeb.ScriptRunner;
using POSHWeb.ScriptRunner.Extensions;
using POSHWeb.Services;
using Serilog;
using SignalRChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// SignalR Setup
builder.Services.AddSignalR();
builder.Services.AddTransient<ScriptHub>();



// Custom
builder.Services.AddServiceCore();
builder.Services.AddScriptRunner();

builder.Services.Configure<POSHWebOptions>(builder.Configuration.GetSection(POSHWebOptions.Section));

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseInMemoryDatabase("ScriptDB");
    options.EnableSensitiveDataLogging();
});

// Add services to the container.

builder.Services.AddHostedService<PSScriptChangeWatcher>();
builder.Services.AddSingleton<ScriptExecuter>();
builder.Services.AddTransient<HasherService>();
builder.Services.AddTransient<PSFileService>();
builder.Services.AddTransient<PSParameterParserService>();
builder.Services.AddTransient<PSScriptValidator>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.IgnoreNullValues = true;
    });

// Application 
builder.Host.UseHostCore();

var app = builder.Build();

app.UseAppCore(builder.Environment);

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ScriptHub>("/signalr/hubs");

app.Run();