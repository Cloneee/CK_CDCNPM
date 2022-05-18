using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Model
{
    public class Storages
    {
        [Key]
        [Required]
        public string StorageId { get; set; }  = string.Empty;
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }

        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;
        public DateTime dateUpdate { get; set; }
        
    }
}
