using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoffeeShop.Model
{
    public class OrderItems
    {
        [Key]
        [Required]
        public string OrderItemId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public Products Product { get; set; }
    }
}
