using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Model;
using CoffeeShop.DTO;

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext dataContext;

        public ProductController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Products>>> GetAll()
        {
            return Ok(await dataContext.Products.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetById(string id)
        {
            var dbProduct = await dataContext.Products.FindAsync(id);

            if (dbProduct == null)
            {
                return BadRequest("Products not found");
            }

            return Ok(dbProduct);
        }

        [HttpPost]
        public async Task<ActionResult<List<Products>>> AddProduct(ProductDTO products)
        {
            var dbCategory = await dataContext.Categories.FindAsync(products.CategoryId);

            var newProductId = "PRO" + AutoGenerateId();
            var checkId = await dataContext.Products.FindAsync(newProductId);
            if (checkId != null)
            {
                while (checkId != null)
                {
                    newProductId = "PRO" + AutoGenerateId();
                    checkId = await dataContext.Products.FindAsync(newProductId);
                }
            }
            if (dbCategory == null)
                return NotFound();

            var newProduct = new Products
            {
                ProductId = newProductId,
                Name = products.Name,
                Description = products.Description,
                Images = products.Images,
                Price = products.Price,
                CategoryId = products.CategoryId,
                IsFeatured = products.IsFeatured,
                dateCreated = DateTime.Now
            };

            dataContext.Products.Add(newProduct);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Products.ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Products>> UpdateProduct(ProductDTO request, string id)
        {
            var dbProduct = await dataContext.Products.FindAsync(id);
            if (dbProduct == null)
            {
                return BadRequest("Products not found");
            }

            dbProduct.Name = request.Name;
            dbProduct.Description = request.Description;
            dbProduct.Images = request.Images;
            dbProduct.Price = request.Price;
            dbProduct.CategoryId = request.CategoryId;
            dbProduct.IsFeatured = request.IsFeatured;
            dbProduct.dateCreated = DateTime.Now;

            await dataContext.SaveChangesAsync();
            return Ok(await dataContext.Products.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductById(string id)
        {
            var dbProduct = await dataContext.Products.FindAsync(id);
            if (dbProduct == null)
            {
                return BadRequest("Products not found");
            }

            dataContext.Products.Remove(dbProduct);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Products.ToListAsync());
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
