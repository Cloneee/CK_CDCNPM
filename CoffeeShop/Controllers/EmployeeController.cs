using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Model;
using CoffeeShop.DTO;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IConfiguration configuration;
        public EmployeeController(DataContext dataContext, IConfiguration configuration)
        {
            this.dataContext = dataContext;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employees>>> GetAll()
        {
            return Ok(await dataContext.Employees.Select(x => new { x.EmployeeId, x.Name, x.Phone, x.Email, x.Address, x.Salary, x.Role }).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employees>> GetById(string id)
        {
            var dbEmployee = await dataContext.Employees.FindAsync(id);
            if (dbEmployee == null)
            {
                return BadRequest("Employees not found");
            }
            return Ok(dbEmployee);
        }

        [HttpPost("Admin/RegisterStaff")]
        public async Task<ActionResult<List<Employees>>> AddEmployee(EmployeeDTO request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordKey);

            var newEmployee = new Employees
            {
                EmployeeId = request.EmplopyeeId,
                Name = request.Name,
                Phone = request.Phone,
                Address = request.Address,
                Salary = request.Salary,
                Email = request.Email,
                Password = passwordHash,
                PasswordKey = passwordKey,
                Role = request.Role
            };

            dataContext.Employees.Add(newEmployee);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Employees.ToListAsync());
        }

        [HttpPut("Admin/UpdateStaff/{id}")]
        public async Task<ActionResult<Employees>> UpdateEmployee(Employees request)
        {
            var dbEmployee = await dataContext.Employees.FindAsync(request.EmployeeId);
            if (dbEmployee == null)
            {
                return BadRequest("Employees not found");
            }

            dbEmployee.Salary = request.Salary;
            dbEmployee.Role = request.Role;

            await dataContext.SaveChangesAsync();
            return Ok(await dataContext.Employees.ToListAsync());
        }

        [HttpPut("Staff/UpdateProfile")]
        public async Task<ActionResult<Employees>> UpdateProfile(EmployeeDTO request)
        {
            var dbEmployee = await dataContext.Employees.FindAsync(request.EmplopyeeId);
            if (dbEmployee == null)
            {
                return BadRequest("Employees not found");
            }

            dbEmployee.Name = request.Name;
            dbEmployee.Email = request.Email;
            dbEmployee.Address = request.Address;
            dbEmployee.Phone = request.Phone;

            await dataContext.SaveChangesAsync();
            return Ok(await dataContext.Employees.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployeeById(string id)
        {
            var dbEmployee = await dataContext.Employees.FindAsync(id);
            if (dbEmployee == null)
            {
                return BadRequest("Employees not found");
            }

            dataContext.Employees.Remove(dbEmployee);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Employees.ToListAsync());
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordKey)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        [HttpPost("Staff/Login")]
        public async Task<ActionResult<string>> Login(AccountDTO request)
        {
            var employee = dataContext.Employees
                .Where(s => s.Email == request.Username)
                .SingleOrDefault();

            if (employee == null)
                return BadRequest("Employees not found");

            if (employee.Email != request.Username)
            {
                return BadRequest("Invalid username");
            }

            if (!VerifyPasswordHash(request.Password, employee.Password, employee.PasswordKey))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(employee);

            return Ok(new {token = token, role = employee.Role, name = employee.Name});
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordKey)
        {

            using (var hmac = new HMACSHA512(passwordKey))
            {

                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(Employees employees)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim("email", employees.Email),
                new Claim(ClaimTypes.Role, employees.Role),
                new Claim("id", employees.EmployeeId)
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
