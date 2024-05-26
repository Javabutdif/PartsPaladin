using System.ComponentModel.DataAnnotations;

namespace PartsPaladin.Models;

public class Orders
{
    [Key]
    public int? order_id { get; set; }
    public int? customer_id { get; set;}
    [Required]
    public string? order_date { get; set; }
    public string? order_status { get; set; }
    public int order_total { get; set; }
    public Customer Customer { get; set; }
    public ICollection<OrderDetails> OrderDetails { get; set; }
}
