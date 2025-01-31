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
using ApiOrder.Service.Query;
using ApiOrder.Service.ServiceCrud;
using ApiOrder.Service.RabbitMq.Publisher;
using ApiOrder.Service.SignalR;
using System.Collections.Generic;

namespace ApiSale.Controller
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderServicePublisher _publisher;
        private readonly OrderServiceCrud _servicecrud;
        private readonly OrderServiceQuery _query;
        private readonly ILogger<OrderController> _logger;

        private readonly IHubContext<NotificationHub> _hubContext;

        public OrderController(ILogger<OrderController> logger, OrderServicePublisher publisher, OrderServiceCrud servicecrud, OrderServiceQuery query, IHubContext<NotificationHub> hubContext)
        {
            _publisher = publisher; 
            _servicecrud = servicecrud;
            _query = query;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<OrderDto>>> GetAll()
        {
            try
            {
                var orders = await _query.GetAllOrderDto();

                await _hubContext.Clients.All.SendAsync("GetListOrder", orders);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching orders");
                return StatusCode(500, "An error occurred while retrieving orders.");
            }
      
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] Order record)
        {
            if (record == null) 
                return BadRequest("Order is required");

            try
            {
                var createdOrder = await this._servicecrud.CreateOrder(record);
                await this._publisher.NotifyStockAvailable(createdOrder);

                this.GetAll();

                return CreatedAtAction(nameof(GetAll), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating orders");
                return StatusCode(500, "An error occurred while creating orders.");
            }

        }
    }
}

