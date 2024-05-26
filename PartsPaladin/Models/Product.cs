using System.ComponentModel.DataAnnotations;

namespace PartsPaladin.Models;

public class Product
{
    [Key]
    public int? product_id { get; set; } = default(int?);
    [Required]
    public string? product_name { get; set; }
    public string? product_description { get; set; }
    public int product_price { get; set; }
    public int? product_stocks { get; set; }
    public string? product_image { get; set; }
    public ICollection<OrderDetails> OrderDetails { get; set; }
    public ICollection<CartItems> CartItems { get; set; }

}
