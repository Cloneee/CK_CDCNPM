using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Model
{
    public class Categories
    {
        [Key]
        [Required]
        public string CategoryId { get; set; } = string.Empty;
        public string Name { get; set; } = String.Empty;
    }
}
