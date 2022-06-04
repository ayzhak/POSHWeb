using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using POSHWeb.Agent;
using POSHWeb.Agent.Environment;
using POSHWeb.Agent.Services;
using POSHWeb.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);


builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console());

// Add services to the container.
var settings = new Dictionary<string, string>
{
    {"JWT:Issuer", "POSHWeb Agent"},
    {"JWT:Audience", "POSHWeb Execution Environement"},
    {"JWT:Key", "A6NpWm8kg6MAJzjYnLvuH4CNyZdV8Qndfpdd5aiVFGbipbhPb3VBqWTaNPBDg9pF"},
};
builder.Configuration.AddInMemoryCollection(settings);

builder.Services.AddSingleton<EnvironmentDB>();
builder.Services.AddTransient<EnvironmentManager>();
builder.Services.AddTransient<TokenService>();
builder.Services.AddHostedService<Worker>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer((options) =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = settings["JWT:Issuer"],
            ValidAudience = settings["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings["JWT:Key"])),
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc(options => options.EnableDetailedErrors = true);
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseGrpcWeb();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<EnvironementCommunicationService>().EnableGrpcWeb();
});
app.MapControllers();

app.Run();
