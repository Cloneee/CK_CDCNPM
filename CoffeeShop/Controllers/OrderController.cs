﻿using Microsoft.AspNetCore.Http;
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
            var newOrder = new Orders
            {
                OrderId = newOrderId,
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

        [HttpPut("update/{id}")]
        public async Task<ActionResult<Orders>> UpdateOrder(OrdersDTO request, string id)
        {
            var dbOrder = await dataContext.Orders.FindAsync(id);
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
