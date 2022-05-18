using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Model;
using CoffeeShop.DTO;

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DataContext dataContext;

        public OrderController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<Orders>>> GetAll()
        {
            return Ok(await dataContext.Orders.ToListAsync());
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Orders>> GetById(string id)
        {
            var dbOrder = await dataContext.Orders.FindAsync(id);
            if (dbOrder == null)
            {
                return BadRequest("Orders not found");
            }
            return Ok(dbOrder);
        }

        [HttpGet("ViewDetail")]
        public async Task<ActionResult<List<OrderItems>>> ViewOrderDetail(string OrderId)
        {
            var OrderItems = await dataContext.OrderItems
                .Where(c => c.OrderId == OrderId)
                .ToListAsync();

            return OrderItems;
        }

        [HttpPost]
        public async Task<ActionResult<List<Orders>>> AddOrder(OrdersDTO orders)
        {
            var newOrder = new Orders
            {
                OrderId = orders.OrderId,
                shippingAddress = orders.shippingAddress,
                Address = orders.Address,
                Status = orders.Status,
                totalPrice = orders.totalPrice,
                CustomersId = orders.CustomersId,
                EmployeesId = orders.EmployeesId,
                dateOrdered = orders.dateOrdered
            };

            dataContext.Orders.Add(newOrder);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Orders.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<Orders>> UpdateOrder(OrdersDTO request)
        {
            var dbOrder = await dataContext.Orders.FindAsync(request.OrderId);
            if (dbOrder == null)
            {
                return BadRequest("Orders not found");
            }

            dbOrder.shippingAddress = request.shippingAddress;
            dbOrder.Address = request.Address;
            dbOrder.Status = request.Status;
            dbOrder.totalPrice = request.totalPrice;
            dbOrder.CustomersId = request.CustomersId;
            dbOrder.EmployeesId = request.EmployeesId;
            dbOrder.dateOrdered = request.dateOrdered;

            await dataContext.SaveChangesAsync();
            return Ok(await dataContext.Orders.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomerById(string id)
        {
            var dbOrder = await dataContext.Orders.FindAsync(id);
            if (dbOrder == null)
            {
                return BadRequest("Orders not found");
            }

            dataContext.Orders.Remove(dbOrder);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Orders.ToListAsync());
        }
    }
}
