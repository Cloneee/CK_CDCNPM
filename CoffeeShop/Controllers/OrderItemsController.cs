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

        // Giống ViewOrderDetail bên OrderController
        [HttpGet("GetAll/{OrderId}")]
        public async Task<ActionResult<List<OrderItems>>> GetAll(string OrderId)
        {
            var OrderItems = await dataContext.OrderItems
                .Where(c => c.OrderId == OrderId)
                .ToListAsync();

            return OrderItems;
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<OrderItems>> GetById(string id)
        {
            var dbOrderItem = await dataContext.OrderItems.FindAsync(id);
            if (dbOrderItem == null)
            {
                return BadRequest("OrderItem not found"); 
            }
            return dbOrderItem;
        }

        [HttpPost("Add")]
        public async Task<ActionResult<List<OrderItems>>> AddOrderItem(OrderItemDTO request)
        {
            var order = await dataContext.Orders.FindAsync(request.OrderId);
            var product = await dataContext.Products.FindAsync(request.ProductId);
            if (product == null || order == null)
                return NotFound();

            var newItemId = "ITEM" + AutoGenerateId();
            var checkId = await dataContext.OrderItems.FindAsync(newItemId);
            if (checkId != null)
            {
                while (checkId != null)
                {
                    newItemId = "ITEM" + AutoGenerateId();
                    checkId = await dataContext.OrderItems.FindAsync(newItemId);
                }
            }
            var newOrderItem = new OrderItems
            {
                OrderItemId = newItemId,
                Quantity = request.Quantity,
                OrderId = request.OrderId,
                ProductId = request.ProductId
            };

            dataContext.OrderItems.Add(newOrderItem);
            await dataContext.SaveChangesAsync();

            return await GetAll(newOrderItem.OrderId);
        }

        [HttpPut("Update/{id}")]
        public async Task<ActionResult<OrderItems>> UpdateOrderItem(OrderItemDTO request, string id)
        {
            var dbOrderItem = await dataContext.OrderItems.FindAsync(id);
            if (dbOrderItem == null)
            {
                return BadRequest("OrderItems not found");
            }

            dbOrderItem.Quantity = request.Quantity;
            dbOrderItem.ProductId = request.ProductId;
            dbOrderItem.OrderId = request.OrderId;

            await dataContext.SaveChangesAsync();
            return Ok(dbOrderItem);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteOrderItemById(string id)
        {
            var dbOrderItem = await dataContext.OrderItems.FindAsync(id);
            if (dbOrderItem == null)
            {
                return BadRequest("OrderItems not found");
            }

            dataContext.OrderItems.Remove(dbOrderItem);
            await dataContext.SaveChangesAsync();

            return Ok("Delete successful");
        }

        [NonAction]
        public string AutoGenerateId()
        {
            string num = "1234567890";
            int len = num.Length;
            string id = string.Empty;
            int iddigit = 7;
            string finaldigit;

            int getindex;

            for (int i = 0; i < iddigit; i++)
            {
                do
                {
                    getindex = new Random().Next(0, len);
                    finaldigit = num.ToCharArray()[getindex].ToString();
                }
                while (id.IndexOf(finaldigit) != -1);
                id += finaldigit;
            }

            return id;
        }
    }
}
