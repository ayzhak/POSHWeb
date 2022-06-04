using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using POSHWeb.Scheduler;
using POSHWeb.Scheduler.Job;
using POSHWeb.Scheduler.Repository;
using POSHWeb.DAL;
using POSHWeb.Data;
using POSHWeb.Extensions;
using POSHWeb.Options;
using POSHWeb.Services;
using SignalRChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// SignalR Setup
builder.Services.AddSignalR();
builder.Services.AddTransient<ScriptHub>();


// Custom
builder.Services.AddServiceCore();
builder.Services.AddQuartz();

builder.Services.AddGrpc(options => options.EnableDetailedErrors = true);

builder.Services.Configure<POSHWebOptions>(builder.Configuration.GetSection(POSHWebOptions.Section));

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseInMemoryDatabase("ScriptDB");
    options.EnableSensitiveDataLogging();
});

// AddJob services to the container.

builder.Services.AddHostedService<ScriptChangeWatcher>();
builder.Services.AddTransient<HasherService>();
builder.Services.AddTransient<ScriptFSSyncService>();
builder.Services.AddTransient<PSParserService>();
builder.Services.AddTransient<ScriptValidatorService>();
builder.Services.AddTransient<SchedulerRepository, SchedulerRepository>();
builder.Services.AddTransient<UnitOfWork>();
builder.Services.AddTransient<LocalScriptJob>();

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

//app.UseAuthorization();

app.MapControllers();

app.MapHub<ScriptHub>("/signalr/hubs");
app.UseRouting();
app.UseGrpcWeb();
app.UseEndpoints(routeBuilder =>
{
    //routeBuilder.MapGrpcService<ScritpGrpcService>().EnableGrpcWeb();
});


app.Run();