using ApiOrder.Service.Query;
using ApiOrder.Service.RabbitMq.Consumer;
using ApiOrder.Service.RabbitMq.Publisher;
using ApiOrder.Service.ServiceCrud;
using ApiOrder.Service.SignalIr;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DBDevContext>();

builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<RabbitMqService>();
//builder.Services.AddScoped(typeof(IRabbitMQPublisher<>), typeof(RabbitMQPublisher<>));
builder.Services.AddHostedService<ConsumerStockFailedService>();
builder.Services.AddHostedService<ConsumerStockOkService>();
builder.Services.AddHostedService<ConsumerWaitPaymentService>();
builder.Services.AddHostedService<ConsumerPaymentOkService>();

builder.Services.AddTransient<OrderServiceCrud>();
builder.Services.AddTransient<OrderServicePublisher>();
builder.Services.AddTransient<OrderServiceQuery>();

#if DEBUG

#else
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // Porta do container
});
#endif



var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

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

