using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoffeeShop.Model
{
    public class Products
    {
        [Key]
        [Required]
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Images { get; set; } = string.Empty;

        public int Price { get; set; }

        public string CategoryId  { get; set; } // khóa ngoại
        [JsonIgnore]
        [ForeignKey("CategoryId")]
        public Categories Categories { get; set; }

        public Boolean IsFeatured { get; set; }

        public DateTime dateCreated { get; set; }

    }
}
