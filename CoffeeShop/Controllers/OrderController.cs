using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Model;
using CoffeeShop.DTO;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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

        [HttpGet]
        public async Task<ActionResult<List<Orders>>> GetAll()
        {
            return Ok(new {data = dataContext.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .ToList(),
            totalPage = 1,
            currentPage = 1
            });
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Orders>> GetById(string id)
        {
            var dbOrder = dataContext.Orders
                .Include(order => order.OrderItems);
            if (dbOrder == null)
            {
                return NotFound("Orders not found");
            }
            return Ok(dbOrder);
        }
        [HttpPost]
        public async Task<ActionResult<List<Orders>>> CreateOrder(OrdersDTO request)
        {
            var newOrderId = "BILL" + AutoGenerateId();
            var checkId = await dataContext.Orders.FindAsync(newOrderId);
            if (checkId != null)
            {
                while (checkId != null)
                {
                    newOrderId = "BILL" + AutoGenerateId();
                    checkId = await dataContext.Orders.FindAsync(newOrderId);
                }
            }
            List<OrderItems> orderItems = new List<OrderItems>();
            foreach (var item in request.OrderItems)
            {
                var newOrderItemId = "ODIT" + AutoGenerateId();
                var checkOrderItemId = await dataContext.OrderItems.FindAsync(newOrderItemId);
                if (checkOrderItemId != null)
                {
                    while (checkOrderItemId != null)
                    {
                        newOrderItemId = "ODIT" + AutoGenerateId();
                        checkOrderItemId = await dataContext.OrderItems.FindAsync(newOrderId);
                    }
                }
                OrderItems orderItem = new OrderItems();
                orderItem.OrderItemId = newOrderItemId;
                orderItem.Product = dataContext.Products.Find(item.ProductId);
                orderItem.Quantity = item.Quantity;
                orderItems.Add(orderItem);
            }
            string id = GetLoggedUserId();
            Customers customer = dataContext.Customers.Find(request.CustomerId);
            Employees employee = dataContext.Employees.Find(request.EmployeeId);
            var newOrder = new Orders
            {
                OrderId = newOrderId,
                Address = customer.Address,
                OrderItems = orderItems,
                TotalPrice = request.TotalPrice,
                Customer = customer,
                Employee = employee,
                DateOrdered = DateTime.Now
            };
            dataContext.Orders.Add(newOrder);
            await dataContext.SaveChangesAsync();
            return Ok(newOrder);
        }

        // [HttpPut("{id}")]
        // public async Task<ActionResult<Orders>> UpdateOrder(OrdersDTO request, string id)
        // {
        //     var dbOrder = await dataContext.Orders.FindAsync(id);
        //     if (dbOrder == null)
        //     {
        //         return BadRequest("Orders not found");
        //     }

        //     dbOrder.Address = request.Address;
        //     dbOrder.TotalPrice = request.TotalPrice;
        //     dbOrder.CustomersId = request.CustomersId;
        //     dbOrder.EmployeesId = request.EmployeesId;

        //     await dataContext.SaveChangesAsync();
        //     return Ok(dbOrder);
        // }

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
        private string GetLoggedUserId()
        {
            if (!User.Identity.IsAuthenticated)
            {
                throw new AuthenticationException();
            }
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return userId;
        }
    }
}
