namespace CoffeeShop.DTO
{
    public class ProductDTO
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Images { get; set; } = string.Empty;
        public int Price { get; set; }
        public string CategoryId { get; set; }
        public Boolean IsFeatured { get; set; }
        public DateTime dateCreated { get; set; }
    }
}
