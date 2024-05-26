using System.ComponentModel.DataAnnotations;

namespace PartsPaladin.Models;

public class Customer
{
    [Key]
    public int? customer_id { get; set; }
    [Required]
    public string? customer_name { get; set; }
    public string? customer_email { get; set; }
    public string? customer_password { get; set; }
    public string? customer_address { get; set; }
    public string? customer_city { get; set; }
    public ICollection<Orders> Orders { get; set; }
    public ICollection<Cart> Carts { get; set; }


}
