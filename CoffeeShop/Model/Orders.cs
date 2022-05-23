using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoffeeShop.Model
{
    public class Orders
    {
        [Key]
        public String OrderId { get; set; } = String.Empty;
        public String Address {get; set; } = String.Empty;
        public String Status {get; set; } = String.Empty;
        public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
        public int TotalPrice {get; set; }
        public Customers? Customer { get; set; }
        public Employees? Employee { get; set; }
        public DateTime DateOrdered { get; set; }
    }
}
