namespace CoffeeShop.DTO
{
    public class StorageDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime dateUpdate { get; set; }
    }
}
