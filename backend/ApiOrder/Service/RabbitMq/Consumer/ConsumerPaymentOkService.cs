﻿using ApiOrder.Service.Query;
using ApiOrder.Service.ServiceCrud;
using ApiOrder.Service.SignalIr;
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
    public class ConsumerPaymentOkService : BackgroundService
    {
        private readonly RabbitMqService _rabbitMqService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly OrderServiceQuery _query;
        private readonly OrderServiceCrud _servicecrud;

        public ConsumerPaymentOkService(OrderServiceCrud servicecrud, OrderServiceQuery query, RabbitMqService rabbitMqService, IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hubContext)
        {
            _servicecrud = servicecrud;
            _query = query;
            _rabbitMqService = rabbitMqService;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMqService._queueName = "SaleToOrder-PaymentOK-Queue";
            await _rabbitMqService.InitializeService();
            await Task.Delay(2000);


            var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DBDevContext>();

            _rabbitMqService.ReceiveMessages(async (message) =>
            {
                var order = JsonSerializer.Deserialize<Order>(message)!;

                OrderStatusPaymentOk(order, dbContext);
            });

            await Task.CompletedTask;
        }
        
        private async Task OrderStatusPaymentOk(Order order, DBDevContext dbContext)
        {
            //todo create service update crud
            _servicecrud.UpdateStatusPaymentOk(order);
            
            List<OrderDto> list = await this._query.GetAllOrderDto();

            await _hubContext.Clients.All.SendAsync("GetListOrder", list);
        }

    }

}
