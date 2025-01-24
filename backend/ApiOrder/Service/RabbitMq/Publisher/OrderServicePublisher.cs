using ApiOrder.Service.Query;
using ApiOrder.Service.ServiceCrud;
using ApiOrder.Service.SignalIr;
using Microsoft.AspNetCore.SignalR;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ApiOrder.Service.RabbitMq.Publisher
{    
    public class OrderServicePublisher
    {
        public readonly RabbitMqService _rabbitMqService;

        public OrderServicePublisher(RabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        public async Task NotifyStockAvailable(Order record)
        {
            _rabbitMqService._queueName = "OrderToStock-CheckProductAvailable-Queue";
            await _rabbitMqService.InitializeService();            
            _rabbitMqService.SendMessage(record);
        }
    }
}
