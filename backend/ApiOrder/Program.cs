using ApiOrder.Service.Query;
using ApiOrder.Service.RabbitMq.Consumer;
using ApiOrder.Service.RabbitMq.Publisher;
using ApiOrder.Service.ServiceCrud;
using ApiOrder.Service.SignalR;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<DBDevContext>();

builder.Services.AddTransient<OrderServiceCrud>();
builder.Services.AddTransient<OrderServiceQuery>();

builder.Services.AddTransient<RabbitMqService>();

builder.Services.AddTransient<OrderServicePublisher>();

builder.Services.AddHostedService<ConsumerStockFailedService>();
builder.Services.AddHostedService<ConsumerStockOkService>();
builder.Services.AddHostedService<ConsumerWaitPaymentService>();
builder.Services.AddHostedService<ConsumerPaymentOkService>();

builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

