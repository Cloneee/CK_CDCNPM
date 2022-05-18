using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Model;

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly DataContext dataContext;

        public CategoriesController(DataContext dataContext) {
            this.dataContext = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Categories>>> GetAll()
        {
            return Ok(await dataContext.Categories.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categories>> GetById(string id)
        {
            var dbCategories = await dataContext.Categories.FindAsync(id);
            if (dbCategories == null)
            {
                return BadRequest("Categories not found");
            }
            return Ok(dbCategories);
        }

        [HttpPost]
        public async Task<ActionResult<List<Categories>>> AddCategories(Categories categories)
        {

            dataContext.Categories.Add(categories);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Categories.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<Categories>> UpdateCategories(Categories request)
        {
            var dbCategories = await dataContext.Categories.FindAsync(request.CategoryId);
            if (dbCategories == null)
            {
                return BadRequest("Storages not found");
            }

            dbCategories.Name = request.Name;

            await dataContext.SaveChangesAsync();
            return Ok(await dataContext.Categories.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoriesById(string id)
        {
            var dbCategories = await dataContext.Categories.FindAsync(id);
            if (dbCategories == null)
            {
                return BadRequest("Storages not found");
            }

            dataContext.Categories.Remove(dbCategories);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Categories.ToListAsync());
        }
    }
}
