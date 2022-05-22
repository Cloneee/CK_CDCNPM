using CoffeeShop.Model;

namespace CoffeeShop.DTO
{
    public class OrdersDTO
    {
        public string CustomersId { get; set; }
        public string EmployeesId { get; set; }
        public String Address { get; set; } = String.Empty;
        public ICollection<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
        public int totalPrice { get; set; }
    }
}
