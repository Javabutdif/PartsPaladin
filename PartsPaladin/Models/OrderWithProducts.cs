namespace PartsPaladin.Models;

public class OrderWithProducts
{
    public Customer customer {  get; set; }
    public OrderDetails details { get; set; }
    public Orders orders { get; set; }
    public Product product { get; set; }
}
