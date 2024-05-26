using System.ComponentModel.DataAnnotations;

namespace PartsPaladin.Models;

public class Cart
{
    [Key]
    public int? cart_id { get; set; }
    public int? customer_id { get; set; }
    public Customer Customer { get; set; }
    public ICollection<CartItems> CartItems { get; set; }

}
