﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoffeeShop.Model
{
    public class Orders
    {
        [Key]
        public String OrderId { get; set; } = String.Empty;
        public String shippingAddress { get; set; } = String.Empty;
        public String Address {get; set; } = String.Empty;
        public String Status {get; set; } = String.Empty;
        public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
        public int totalPrice {get; set; }
        [Required]
        public string? CustomersId { get; set; } // khóa ngoại
        [ForeignKey("CustomersId")]
        [JsonIgnore]
        public Customers Customers { get; set; }
        [Required]
        public string? EmployeesId   { get; set; } // khóa ngoại
        [ForeignKey("EmployeesId")]
        [JsonIgnore]
        public Employees Employees { get; set; }
        public DateTime dateOrdered { get; set; }
    }
}
