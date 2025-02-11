using ApiStock.Domain.Product.Repository;
using ApiStock.Service;
using ApiStock.Service.Redis;
using ApiStock.Service.SignalR;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DBDevContext>();

//rabbit
builder.Services.AddTransient<RabbitMqService>();

builder.Services.AddHostedService<ConsumerStockAvailableService>();

builder.Services.AddTransient<ProductRepository>();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

#if DEBUG
var apiUrls = builder.Configuration.GetSection("ConnectionStrings").Get<Dictionary<string, string>>();
builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
{
    //return ConnectionMultiplexer.Connect(apiUrls["redis"]);
    return ConnectionMultiplexer.Connect("localhost: 6379");

});
#else
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("redis:6379"));

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // Porta do container
});
#endif
// Redis server

//redis todo get from appsettings
//builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379")); // Redis server

//quando usado docker
//builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("redis:6379")); // Redis server
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors(cors => cors
.AllowAnyMethod()
.AllowAnyHeader()
.AllowAnyOrigin()
);

app.UseCors((g) => g.AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");


app.Run();

