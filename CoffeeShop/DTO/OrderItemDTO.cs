namespace CoffeeShop.DTO
{
    public class OrderItemDTO
    {
        public string OrderItemId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
    }
}
