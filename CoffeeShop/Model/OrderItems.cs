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

        
        public string OrderId { get; set; }
        [JsonIgnore]
        [ForeignKey("OrderId")]
        public Orders Orders { get; set; }

        
        public string ProductId { get; set; } // khóa ngoại
        [JsonIgnore]
        [ForeignKey("ProductId")]
        public Products Products { get; set; }

    }
}
