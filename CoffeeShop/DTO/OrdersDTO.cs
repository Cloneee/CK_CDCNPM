using CoffeeShop.Model;

namespace CoffeeShop.DTO
{
    public class OrdersDTO
    {
        public String OrderId { get; set; }
        public String shippingAddress { get; set; }
        public String Address { get; set; } = String.Empty;
        public String Status { get; set; } = String.Empty;
        public int totalPrice { get; set; }
        public string CustomersId { get; set; }
        public string EmployeesId { get; set; }
        public DateTime dateOrdered { get; set; }
    }
}
