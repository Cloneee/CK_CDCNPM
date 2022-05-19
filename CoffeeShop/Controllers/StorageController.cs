using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Model;
using CoffeeShop.DTO;
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
        public async Task<ActionResult<List<Storages>>> AddStorage(StorageDTO storage) {

            var newStorageId = "STO" + AutoGenerateId();
            var checkId = await dataContext.Storages.FindAsync(newStorageId);
            if (checkId != null)
            {
                while (checkId != null)
                {
                    newStorageId = "STO" + AutoGenerateId();
                    checkId = await dataContext.Storages.FindAsync(newStorageId);
                }
            }
            var newStorage = new Storages {
                StorageId = newStorageId,
                Name = storage.Name,
                Description = storage.Description,
                Quantity = storage.Quantity,
                Unit = storage.Unit,
                dateUpdate = storage.dateUpdate
            };

            dataContext.Storages.Add(newStorage);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Storages.ToListAsync());
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<Storages>> UpdateStorage(StorageDTO request, string id) {
            var dbStorage = await dataContext.Storages.FindAsync(id);
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
