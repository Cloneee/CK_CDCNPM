using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Model;
using CoffeeShop.DTO;

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly DataContext dataContext;

        public OrderItemsController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderItems>>> GetAll(string OrderId)
        {
            var OrderItems = await dataContext.OrderItems
                .Where(c => c.OrderId == OrderId)
                .ToListAsync();

            return OrderItems;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItems>> GetById(string id)
        {
            var dbOrderItem = await dataContext.OrderItems.FindAsync(id);
            if (dbOrderItem == null)
            {
                return BadRequest("OrderItem not found"); 
            }
            return dbOrderItem;
        }

        [HttpPost("AddOrderItem")]
        public async Task<ActionResult<List<OrderItems>>> AddOrderItem(OrderItemDTO request)
        {
            var order = await dataContext.Orders.FindAsync(request.OrderId);
            var product = await dataContext.Products.FindAsync(request.ProductId);
            if (product == null || order == null)
                return NotFound();
            
            var newOrderItem = new OrderItems
            {
                OrderItemId = request.OrderItemId,
                Quantity = request.Quantity,
                OrderId = request.OrderId,
                ProductId = request.ProductId
            };

            dataContext.OrderItems.Add(newOrderItem);
            await dataContext.SaveChangesAsync();

            return await GetAll(newOrderItem.OrderId);
        }

        [HttpPut]
        public async Task<ActionResult<OrderItems>> UpdateOrderItem(OrderItemDTO request)
        {
            var dbOrderItem = await dataContext.OrderItems.FindAsync(request.OrderItemId);
            if (dbOrderItem == null)
            {
                return BadRequest("OrderItems not found");
            }

            dbOrderItem.Quantity = request.Quantity;
            dbOrderItem.ProductId = request.ProductId;
            dbOrderItem.OrderId = request.OrderId;
            dbOrderItem.ProductId = request.ProductId;

            await dataContext.SaveChangesAsync();
            return Ok(await dataContext.OrderItems.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrderItemById(string id)
        {
            var dbOrderItem = await dataContext.OrderItems.FindAsync(id);
            if (dbOrderItem == null)
            {
                return BadRequest("OrderItems not found");
            }

            dataContext.OrderItems.Remove(dbOrderItem);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.OrderItems.ToListAsync());
        }
    }
}
