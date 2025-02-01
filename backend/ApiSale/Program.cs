using ApiSale.Controller;
using ApiSale.Service;
using ApiSale.Service.Query;
using ApiSale.Service.SignalR;
using Microsoft.EntityFrameworkCore;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using StackExchange.Redis;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddDbContext<DBDevContext>();
builder.Services.AddTransient<SaleQueryService>();


builder.Services.AddTransient<RabbitMqService>();
//builder.Services.AddScoped(typeof(IRabbitMQPublisher<>), typeof(RabbitMQPublisher<>));
builder.Services.AddHostedService<ConsumerStockOkService>();


#if DEBUG

#else
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("redis:6379"));

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // Porta do container
});
#endif


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
app.UseCors((g) => g.AllowCredentials());

app.UseHttpsRedirection();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();

