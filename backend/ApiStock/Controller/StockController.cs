using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Polly;

namespace ApiStock.Controller
{
    //process
    //1 action => create status = create | send sale create
    //2 action => camcel => check sale create and delete sale e cancel order

    [ApiController]
    [Route("api/v1/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly DBDevContext _context;
        public readonly RabbitMqService _rabbitMqService;

        public StockController(RabbitMqService rabbitMqService, DBDevContext context)
        {
            _rabbitMqService = rabbitMqService;         
            _context = context;
        }

        //test
        [HttpGet("get")]
        public async Task<ActionResult<string>> Get()
        {

            return Ok("ok");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]        
        public async Task CheckAvailability(Order order)
        {
            await this.Check(order);                                            
        }

        //todo return message to client amout available for the product
        //todo reverve quantiy in stock
        private async Task Check(Order order)
        {
            Stock stock = await _context.Stock.SingleAsync(x => x.Idproduct == order.Idproduct);
            if (stock != null && stock.Amount >= order.Amount)
            {
                this.NotifyOrderOk(order);
                this.NotifySaleOk(order);
            }
            else
            {
                NotifyOrderFailed(order);
            }
        }

        private async Task NotifySaleOk(Order order)
        {
            _rabbitMqService._queueName = "StockToSale-StockOK-Queue";
            await _rabbitMqService.InitializeService();            
            _rabbitMqService.SendMessage(order);            
        }

        private async Task NotifyOrderOk(Order order)
        {
            _rabbitMqService._queueName = "StockToOrder-StockOK-Queue";
            await _rabbitMqService.InitializeService();            
            _rabbitMqService.SendMessage(order);            
        }

        //todo create DTO and send amout of product in stock
        //private async Task NotifyOrderFaile(int amoutproduct, int amoutstock)
        private async Task NotifyOrderFailed(Order order)
        {
            _rabbitMqService._queueName = "StockToOrder-StockFailed-Queue";
            await _rabbitMqService.InitializeService();            
            _rabbitMqService.SendMessage(order);            
        }
    }
}

