using System.ComponentModel.DataAnnotations;

namespace PartsPaladin.Models;

public class CartItems
{
    [Key]
    public int? cart_item_id { get; set; }
    public int? cart_id { get; set; }
    public int? product_id { get; set; }
    public int? quantity { get; set; }
    public int subtotal { get; set; }
    public Cart Cart { get; set; }
    public Product Product { get; set; }
}
