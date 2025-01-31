using ApiSale.Service.Query;
using ApiSale.Service.SignalR;
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
        private readonly SaleQueryService _query;


        public SaleController(SaleQueryService query, RabbitMqService rabbitMqService, DBDevContext context, IHubContext<NotificationHub> hubContext)
        {
            _query = query;
            _rabbitMqService = rabbitMqService;
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<SaleDto>>> GetAll()
        {
            return Ok(await this._query.GetAllSaleDto());
        }


        [HttpPost]
        public async Task<ActionResult<Sale>> PaymentSale([FromBody] int idsale)
        {
            try
            {
                Sale sale =  await this.SaleStatusPayment(idsale, _context);
                if (sale != null)
                {
                    Order order = await _context.Order.SingleAsync(x => x.Id == sale.Idorder);
                    if (order != null)
                    {
                        await this.NotifyOrderPaymentOk(order);
                        await this._query.GetAllSaleDto();
                    }

                    return Ok(sale);
                }

                return NotFound("Sale not found Sale");
            }
            catch (Exception ex)
            {
                return BadRequest("Payment Sale => " + ex); 
                throw ex;
            }
           
        }

        private async Task<Sale> SaleStatusPayment(int idsale, DBDevContext context)
        {
            Sale sale = await context.Sale.SingleOrDefaultAsync(x => x.Id == idsale);
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

