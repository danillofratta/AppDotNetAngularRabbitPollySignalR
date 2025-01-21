using ApiStock.Domain.Repository;
using ApiStock.Service;
using ApiStock.Service.Redis;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddDbContext<DBDevContext>();

//rabbit
builder.Services.AddTransient<RabbitMqService>();
//builder.Services.AddScoped(typeof(IRabbitMQPublisher<>), typeof(RabbitMQPublisher<>));
builder.Services.AddHostedService<ConsumerStockAvailableService>();

builder.Services.AddTransient<ProductRepository>();

//redis todo get from appsettings
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379")); // Redis server
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//mediaR 
//builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
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


app.Run();

