using ApiOrder.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SharedDatabase.Dto;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;

namespace ApiSale.Controller
{
    //process    
    //1 action => process (separando estoque)
    //2 action => delivery => send order
    //3 action => deliverad => send order
    //4 action => done => send order
    //5 action => cancel delete sale and send order    

    [ApiController]
    [Route("api/v1/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly DBDevContext _context;
        public readonly RabbitMqService _rabbitMqService;
        private readonly IHubContext<NotificationHub> _hubContext;


        public SaleController(RabbitMqService rabbitMqService, DBDevContext context, IHubContext<NotificationHub> hubContext)
        {
            _rabbitMqService = rabbitMqService;
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<SaleDto>>> GetAll()
        {
            return Ok(await this.GetAllSaleDto());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public async Task<List<SaleDto>> GetAllSaleDto()
        {
            var list = (from a in _context.Sale
                        join b in _context.Status on a.Idstatus equals b.Id
                        join c in _context.Product on a.Idproduct equals c.Id
                        select new SharedDatabase.Dto.SaleDto
                        {
                            Id = a.Id,
                            Idproduct = a.Idproduct,
                            Idcustomer = a.Idcustomer,
                            Idstatus = a.Idstatus,
                            Idorder = a.Idorder,
                            Price = a.Price,
                            Amount = a.Amount,
                            CreateAt = a.CreateAt,
                            namestatus = b.Name,
                            nameproduct = c.Name
                        }).ToList<SaleDto>();


            await _hubContext.Clients.All.SendAsync("GetListSale", list);

            return list;
        }

        [HttpPost("PaymentOK")]
        public async Task<ActionResult<Sale>> PaymentSale([FromBody] int idsale)
        {
            try
            {
                Sale sale =  await this.SaleStatusPayment(idsale, _context);

                Order order = await _context.Order.SingleAsync(x => x.Id == sale.Idorder);
                if (order != null)
                {
                    await this.NotifyOrderPaymentOk(order);
                    await this.GetAllSaleDto();
                }

                return Ok(sale);
            }
            catch (Exception ex)
            {
                return BadRequest("Pament Sale => " + ex); 
                throw ex;
            }
           
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<Sale> SaleStatusPayment(int idsale, DBDevContext context)
        {
            Sale sale = await context.Sale.SingleAsync(x => x.Id == idsale);
            if (sale != null){
                sale.Idstatus = 8;
                _context.Sale.Update(sale);
                await _context.SaveChangesAsync();
                return sale;
            }

            return null;
        }

        private async Task NotifyOrderPaymentOk(Order order)
        {
            _rabbitMqService._queueName = "SaleToOrder-PaymentOK-Queue";
            await _rabbitMqService.InitializeService();            
            await _rabbitMqService.SendMessage(order);            
        }
    }
}

