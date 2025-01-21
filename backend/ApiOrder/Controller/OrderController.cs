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
using ApiOrder.Service;
using Microsoft.AspNetCore.SignalR;

namespace ApiSale.Controller
{
    //process
    //1 action => create status = create | send sale create
    //2 action => camcel => check sale create and delete sale e cancel order

    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly DBDevContext _context;
        public readonly RabbitMqService _rabbitMqService;

        private readonly IHubContext<NotificationHub> _hubContext;

        public OrderController(RabbitMqService rabbitMqService, DBDevContext context, IHubContext<NotificationHub> hubContext)
        {
            _rabbitMqService = rabbitMqService;
            _context = context;
            _hubContext = hubContext;
        }

        //public OrderController(RabbitMqService rabbitMqService, DBDevContext context)
        //{
        //    _rabbitMqService = rabbitMqService;
        //    _context = context;
        //}

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<OrderDto>>> GetAll()
        {
            return Ok(await this.GetAllOrderDto());       
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public async Task<List<OrderDto>> GetAllOrderDto()
        {
            //todo nameproduct namestatus 
            //todo adddto
            //todo add foreign key e add include
            //List<SharedDatabase.Dto.OrderDto> list = _context.Order.ToList();
            var list = await (from a in _context.Order
                        join b in _context.Status on a.Idstatus equals b.Id
                        join c in _context.Product on a.Idproduct equals c.Id
                        select new SharedDatabase.Dto.OrderDto
                        {
                            Id = a.Id,
                            Idproduct = a.Amount,
                            Idcustomer = a.Idcustomer,
                            Idstatus = a.Idstatus,
                            Price = a.Price,
                            Amount = a.Amount,
                            CreateAt = a.CreateAt,
                            namestatus = b.Name,
                            nameproduct = c.Name
                        }).ToListAsync<OrderDto>();

            return list;
        }

        //to try catch
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] Order record)
        {
            if (record == null)
                return BadRequest("Order is required");

            record = await this.CreateOrdemDB(record);
            await this.NotifyStockAvailable(record);

            return Ok("ok");// CreatedAtAction("GetOrder", new { id = record.Id }, record);
        }

        //todo transacation rollback if fail rabbit
        private async Task<Order> CreateOrdemDB(Order record)
        {
            record.Idstatus = 1;
            _context.Order.Add(record);
            await _context.SaveChangesAsync();

            return record;
        }

        private async Task NotifyStockAvailable(Order record)
        {
            await _rabbitMqService.InitializeService();
            _rabbitMqService._queueName = "OrderToStock-CheckProductAvailable-Queue";
            _rabbitMqService.SendMessage(record);
        }       
    }
}

