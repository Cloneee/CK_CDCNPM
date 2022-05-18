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

            if (dbCategory == null)
                return NotFound();

            var newProduct = new Products
            {
                ProductId = products.ProductId,
                Name = products.Name,
                Description = products.Description,
                Images = products.Images,
                Price = products.Price,
                CategoryId = products.CategoryId,
                IsFeatured = products.IsFeatured,
                dateCreated = products.dateCreated
            };

            dataContext.Products.Add(newProduct);
            await dataContext.SaveChangesAsync();

            return Ok(await dataContext.Products.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<Products>> UpdateProduct(ProductDTO request)
        {
            var dbProduct = await dataContext.Products.FindAsync(request.ProductId);
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
            dbProduct.dateCreated = request.dateCreated;

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

        // API upload hình ảnh
    }
}
