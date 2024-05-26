using System.ComponentModel.DataAnnotations;

namespace PartsPaladin.Models;

public class OrderDetails
{
    [Key]
    public int? order_detail_id { get; set; }
    public int? order_id { get; set; }
    public int? product_id { get; set; }

    [Required]
    public int? quantity { get; set; }
    public int price { get; set; }
    public Orders Order { get; set; }
    public Product Product { get; set; }
}
