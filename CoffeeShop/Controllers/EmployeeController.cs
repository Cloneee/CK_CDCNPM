﻿using Microsoft.AspNetCore.Http;
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
            var newEmId = "EM" + AutoGenerateId();
            var checkId = await dataContext.Employees.FindAsync(newEmId);
            if (checkId != null)
            {
                while (checkId != null)
                {
                    newEmId = "EM" + AutoGenerateId();
                    checkId = await dataContext.Employees.FindAsync(newEmId);
                }
            }
            var newEmployee = new Employees
            {
                EmployeeId = newEmId,
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
        public async Task<ActionResult<Employees>> UpdateEmployee(SalaryNRoleDTO request, string id)
        {
            var dbEmployee = await dataContext.Employees.FindAsync(id);
            if (dbEmployee == null)
            {
                return BadRequest("Employees not found");
            }

            dbEmployee.Salary = request.Salary;
            dbEmployee.Role = request.Role;

            await dataContext.SaveChangesAsync();
            return Ok(await dataContext.Employees.ToListAsync());
        }

        [HttpPut("Staff/UpdateProfile/{id}")]
        public async Task<ActionResult<Employees>> UpdateProfile(StaffProfileDTO request, string id)
        {
            var dbEmployee = await dataContext.Employees.FindAsync(id);
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

        [HttpPost("Employee/ChangePassword")]
        public async Task<ActionResult<Customers>> ChangePassword(ChangePasswordDTO request)
        {

            var employee = dataContext.Employees
                .Where(s => s.Email == request.Username)
                .SingleOrDefault();

            if (employee == null)
                return BadRequest("Employee not found");

            if (employee.Email != request.Username)
            {
                return BadRequest("Invalid username");
            }
            if (!VerifyPasswordHash(request.currentPassword, employee.Password, employee.PasswordKey))
            {
                return BadRequest("Wrong password");
            }
            if (request.confirmPassword != request.newPassword)
            {
                return BadRequest("confirm password does not match");
            }
            CreatePasswordHash(request.confirmPassword, out byte[] passwordHash, out byte[] passwordKey);

            employee.Password = passwordHash;
            employee.PasswordKey = passwordKey;

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
