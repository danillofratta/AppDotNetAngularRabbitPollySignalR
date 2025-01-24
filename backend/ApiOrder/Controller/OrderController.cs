using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Polly;
using SharedDatabase.Dto;
using Microsoft.AspNetCore.SignalR;
using ApiOrder.Service.SignalIr;
using ApiOrder.Service.Query;
using ApiOrder.Service.ServiceCrud;
using ApiOrder.Service.RabbitMq.Publisher;

namespace ApiSale.Controller
{
    //process
    //1 action => create status = create | send sale create
    //2 action => camcel => check sale create and delete sale e cancel order

    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderServicePublisher _publisher;
        private readonly OrderServiceCrud _servicecrud;
        private readonly OrderServiceQuery _query;
        private readonly DBDevContext _context;

        private readonly IHubContext<NotificationHub> _hubContext;

        public OrderController(OrderServicePublisher publisher, OrderServiceCrud servicecrud, OrderServiceQuery query, DBDevContext context, IHubContext<NotificationHub> hubContext)
        {
            _publisher = publisher; 
            _servicecrud = servicecrud;
            _query = query;
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<OrderDto>>> GetAll()
        {
            return Ok(await this._query.GetAllOrderDto());       
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] Order record)
        {
            if (record == null)
                return BadRequest("Order is required");

            record = await this._servicecrud.CreateOrdem(record);
            await this._publisher.NotifyStockAvailable(record);

            return Ok("ok");
        }
    }
}

