using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Model;

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly DataContext dataContext;

        public StorageController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Storages>>> GetAll() {
            return Ok(await dataContext.Storages.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Storages>> GetById(string id) {
            var storage = await dataContext.Storages.FindAsync(id);
            if (storage == null) {
                return BadRequest("Storages not found");
            }
            return Ok(storage);
        }

        [HttpPost]
        public async Task<ActionResult<List<Storages>>> AddStorage(Storages storage) {

            dataContext.Storages.Add(storage);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Storages.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<Storages>> UpdateStorage(Storages request) {
            var dbStorage = await dataContext.Storages.FindAsync(request.StorageId);
            if (dbStorage == null)
            {
                return BadRequest("Storages not found");
            }

            dbStorage.Name = request.Name;
            dbStorage.Description = request.Description;
            dbStorage.Quantity = request.Quantity;
            dbStorage.Unit = request.Unit;
            dbStorage.dateUpdate = request.dateUpdate;

            await dataContext.SaveChangesAsync();
            return Ok(await dataContext.Storages.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStorageById(string id) {
            var dbStorage = await dataContext.Storages.FindAsync(id);
            if (dbStorage == null)
            {
                return BadRequest("Storages not found");
            }

            dataContext.Storages.Remove(dbStorage);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Storages.ToListAsync());
        }
    }
}
