﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Model;
using System.Security.Cryptography;
using CoffeeShop.DTO;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IConfiguration configuration;
        public CustomerController(DataContext dataContext, IConfiguration configuration)
        {
            this.dataContext = dataContext;
            this.configuration = configuration;
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

        private string CreateToken(Customers customers)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, customers.Email),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordKey)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordKey)
        {

            using (var hmac = new HMACSHA512(passwordKey))
            {

                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult<List<Customers>>> Register(CustomerDTO request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordKey);
            var newCusId = "CUS" + AutoGenerateId();
            var checkId = await dataContext.Customers.FindAsync(newCusId);
            if (checkId != null)
            {
                while (checkId == null)
                {
                    newCusId = "CUS" + AutoGenerateId();
                    checkId = await dataContext.Customers.FindAsync(newCusId);
                }
            }

            var checkEmail = dataContext.Customers
                .Where(s => s.Email == request.Email)
                .SingleOrDefault();
            if (checkEmail != null)
            {
                return BadRequest("Email này đã tồn tại. Xin vui lòng đăng ký bằng Email khác");
            }

            var newCustomer = new Customers
            {
                CustomerId = newCusId,
                Name = request.Name,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                Password = passwordHash,
                PasswordKey = passwordKey,
                Role = "Customer"
            };

            dataContext.Customers.Add(newCustomer);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Customers.ToListAsync());
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(AccountDTO request)
        {
            var customer = dataContext.Customers
                .Where(s => s.Email == request.Username)
                .SingleOrDefault();

            if (customer == null)
                return BadRequest("Customer not found");

            if (customer.Email != request.Username)
            {
                return BadRequest("Invalid username");
            }

            if (!VerifyPasswordHash(request.Password, customer.Password, customer.PasswordKey))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(customer);

            return Ok(token);
        }

        [HttpPost("ChangePassword"), Authorize(Roles = "Customer")]
        public async Task<ActionResult<Customers>> ChangePassword(ChangePasswordDTO request)
        {

            var customer = dataContext.Customers
                .Where(s => s.Email == request.Username)
                .SingleOrDefault();

            if (customer == null)
                return BadRequest("Customer not found");

            if (customer.Email != request.Username)
            {
                return BadRequest("Invalid username");
            }
            if (!VerifyPasswordHash(request.currentPassword, customer.Password, customer.PasswordKey))
            {
                return BadRequest("Wrong password");
            }
            if (request.confirmPassword != request.newPassword)
            {
                return BadRequest("confirm password does not match");
            }
            CreatePasswordHash(request.confirmPassword, out byte[] passwordHash, out byte[] passwordKey);

            customer.Password = passwordHash;
            customer.PasswordKey = passwordKey;

            await dataContext.SaveChangesAsync();
            return Ok(await dataContext.Customers.ToListAsync());
        }

        [HttpGet("GetAllCustomer")]
        public async Task<ActionResult<List<Customers>>> GetAll()
        {
            return Ok(await dataContext.Customers.ToListAsync());
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Customers>> GetById(string id)
        {
            var dbCustomer = await dataContext.Customers.FindAsync(id);
            if (dbCustomer == null)
            {
                return BadRequest("Customers not found");
            }
            return Ok(dbCustomer);
        }

        [HttpPut("updateProfile/{id}"), Authorize(Roles = "Customer")]
        public async Task<ActionResult<Customers>> UpdateCustomer(CustomerProfileDTO request, string id)
        {
            var dbCustomer = await dataContext.Customers.FindAsync(id);
            if (dbCustomer == null)
            {
                return BadRequest("Customers not found");
            }

            dbCustomer.Name = request.Name;
            dbCustomer.Email = request.Email;
            dbCustomer.Phone = request.Phone;
            dbCustomer.Address = request.Address;

            await dataContext.SaveChangesAsync();
            return Ok(await dataContext.Customers.ToListAsync());
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteCustomerById(string id)
        {
            var dbCustomer = await dataContext.Customers.FindAsync(id);
            if (dbCustomer == null)
            {
                return BadRequest("Customers not found");
            }

            dataContext.Customers.Remove(dbCustomer);
            await dataContext.SaveChangesAsync();

            return Ok("Delete successful");
        }

    }
}
