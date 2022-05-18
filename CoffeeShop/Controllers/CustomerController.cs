using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Model;
using System.Security.Cryptography;
using CoffeeShop.DTO;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

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

        [HttpGet]
        public async Task<ActionResult<List<Customers>>> GetAll()
        {
            return Ok(await dataContext.Customers.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customers>> GetById(string id)
        {
            var dbCustomer = await dataContext.Customers.FindAsync(id);
            if (dbCustomer == null)
            {
                return BadRequest("Customers not found");
            }
            return Ok(dbCustomer);
        }

        [HttpPost("Staff/RegisterCustomer")]
        public async Task<ActionResult<List<Customers>>> AddCustomer(CustomerDTO request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordKey);

            var newCustomer = new Customers {
                CustomerId = request.CustomerId,
                Name = request.Name,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                Password = passwordHash,
                PasswordKey = passwordKey,
                Role = request.Role
            };

            dataContext.Customers.Add(newCustomer);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Customers.ToListAsync());
        }

        [HttpPost("Register")]
        public async Task<ActionResult<List<Customers>>> Register(CustomerDTO request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordKey);

            var newCustomer = new Customers
            {
                CustomerId = request.CustomerId,
                Name = request.Name,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                Password = passwordHash,
                PasswordKey = passwordKey,
                Role = "NormalCus"
            };

            dataContext.Customers.Add(newCustomer);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Customers.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<Customers>> UpdateCustomer(Customers request)
        {
            var dbCustomer = await dataContext.Customers.FindAsync(request.CustomerId);
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomerById(string id)
        {
            var dbCustomer = await dataContext.Customers.FindAsync(id);
            if (dbCustomer == null)
            {
                return BadRequest("Customers not found");
            }

            dataContext.Customers.Remove(dbCustomer);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Customers.ToListAsync());
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordKey)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordKey) {

            using (var hmac = new HMACSHA512(passwordKey)) {

                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
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

            if (!VerifyPasswordHash(request.Password, customer.Password, customer.PasswordKey)) {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(customer);

            return Ok(new {token = token, name = customer.Name});
        }

        private string CreateToken(Customers customers) {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, customers.Email)
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
    }
}
