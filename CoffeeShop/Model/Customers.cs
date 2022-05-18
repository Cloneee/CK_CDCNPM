using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoffeeShop.Model
{
    public class Customers
    {
        [Key]
        [Required]
        public string CustomerId { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
        [StringLength(10)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(50)]
        public string Email { get; set; } = string.Empty;
        public byte[] Password { get; set; }
        public byte[] PasswordKey { get; set; }
        public string Role { get; set; } = string.Empty;

    }
}
