namespace PartsPaladin.Models;

public class CartWithProducts
{
    public Cart cart { get; set; }
    public CartItems cartItems { get; set; }
    public Product Product { get; set; }
}
