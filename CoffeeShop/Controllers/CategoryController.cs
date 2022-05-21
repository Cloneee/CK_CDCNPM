using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Model;
using CoffeeShop.DTO;

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext dataContext;

        public CategoryController(DataContext dataContext) {
            this.dataContext = dataContext;
        }

        [HttpGet()]
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
        public async Task<ActionResult<List<Categories>>> AddCategories(CategoryDTO request)
        {
            var newCatId = "CAT" + AutoGenerateId();
            var checkId = await dataContext.Categories.FindAsync(newCatId);
            if (checkId != null)
            {
                while (checkId != null)
                {
                    newCatId = "CAT" + AutoGenerateId();
                    checkId = await dataContext.Categories.FindAsync(newCatId);
                }
            }
            var newCatergory = new Categories{
                CategoryId = newCatId,
                Name = request.Name
            };

            dataContext.Categories.Add(newCatergory);
            await dataContext.SaveChangesAsync();

            return Ok(newCatergory);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Categories>> UpdateCategories(CategoryDTO request, string id)
        {
            var dbCategories = await dataContext.Categories.FindAsync(id);
            if (dbCategories == null)
            {
                return BadRequest("Storages not found");
            }

            dbCategories.Name = request.Name;

            await dataContext.SaveChangesAsync();
            return Ok(dbCategories);
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
