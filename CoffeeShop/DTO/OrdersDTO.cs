using CoffeeShop.Model;

namespace CoffeeShop.DTO
{
    public class OrdersDTO
    {
        public string CustomerId { get; set; } = String.Empty;
        public string EmployeeId { get; set; } = String.Empty;
        public String Address { get; set; } = String.Empty;
        public ICollection<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
        public int TotalPrice { get; set; } = 0;
    }
}
