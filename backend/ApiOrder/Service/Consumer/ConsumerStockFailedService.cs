using ApiSale.Controller;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedDatabase.Dto;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace ApiOrder.Service
{
    public class ConsumerStockFailedService : BackgroundService
    {
        private readonly RabbitMqService _rabbitMqService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ConsumerStockFailedService(RabbitMqService rabbitMqService, IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hubContext)
        {
            _rabbitMqService = rabbitMqService;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            //_rabbitMqService.InitializeService();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMqService._queueName = "StockToOrder-StockFailed-Queue";
            await _rabbitMqService.InitializeService();
            await Task.Delay(2000);
            

            //while (!stoppingToken.IsCancellationRequested)
            //{
                var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DBDevContext>();

                _rabbitMqService.ReceiveMessages(async (message) =>
                {
                    var order = JsonSerializer.Deserialize<Order>(message)!;

                    this.OrderStatusFailed(order, dbContext);
                });
            //    await Task.Delay(1000, stoppingToken);
            //}

            await Task.CompletedTask;
        }

        //todo put in core domain service
        private async Task OrderStatusFailed(Order order, DBDevContext dbContext)
        {
            order.Idstatus = 6;
            dbContext.Order.Update(order);
            await dbContext.SaveChangesAsync();

            OrderController oOrderController = new OrderController(_rabbitMqService, dbContext, _hubContext);
            List<OrderDto> list = await oOrderController.GetAllOrderDto();


            await _hubContext.Clients.All.SendAsync("GetListOrder", list);
        }

    }

}
