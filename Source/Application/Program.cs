using Application;
using Application.Services;
using SolaceNEMS.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var appConfig = builder.Configuration.GetSection("AppConfig");
builder.Services.Configure<AppConfig>(appConfig);

var config = builder.Configuration.GetSection("SolaceNEMS");
builder.Services.AddNEMS(config);

builder.Services.AddSingleton<EventLoader>();
builder.Services.AddHostedService<SubscriberService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
