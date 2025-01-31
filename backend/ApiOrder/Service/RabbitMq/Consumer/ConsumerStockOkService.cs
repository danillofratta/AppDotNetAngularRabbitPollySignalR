using ApiOrder.Enun;
using ApiOrder.Service.Query;
using ApiOrder.Service.ServiceCrud;
using ApiOrder.Service.SignalR;
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

namespace ApiOrder.Service.RabbitMq.Consumer
{
    public class ConsumerStockOkService : BackgroundService
    {
        private readonly RabbitMqService _rabbitMqService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly OrderServiceQuery _query;
        private readonly OrderServiceCrud _servicecrud;

        public ConsumerStockOkService(OrderServiceCrud servicecrud, OrderServiceQuery query, RabbitMqService rabbitMqService, IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hubContext)
        {
            _servicecrud = servicecrud;
            _query = query;
            _rabbitMqService = rabbitMqService;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMqService._queueName = "StockToOrder-StockOK-Queue";
            await _rabbitMqService.InitializeService();

            var scope = _scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DBDevContext>();

            await _rabbitMqService.ReceiveMessages(async (message) =>
            {
                var order = JsonSerializer.Deserialize<Order>(message)!;

                await OrderStatusProcess(order, dbContext);
            });
        }

        private async Task OrderStatusProcess(Order order, DBDevContext dbContext)
        {
            await _servicecrud.UpdateOrderStatusAsync(order, OrderStatus.StockOk);
        }

    }

}
