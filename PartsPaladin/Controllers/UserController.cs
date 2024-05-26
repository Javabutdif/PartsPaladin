using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PartsPaladin.Data;
using PartsPaladin.Models;
using System.Linq;
using System.Reflection.Metadata;

namespace PartsPaladin.Controllers
{
    public class UserController(PartsPaladinContext context) : Controller
    {
        private readonly PartsPaladinContext _context = context;
        public IActionResult Index()
        {
            return View();
        }
        public async Task<List<CartWithProducts>> GetCartAsync()
        {
            var customer_id = HttpContext.Session.GetInt32("id");

            var query = from customer in _context.Customer
                        join cart in _context.Cart on customer.customer_id equals cart.customer_id
                        join cartItem in _context.CartItems on cart.cart_id equals cartItem.cart_id
                        join product in _context.Product on cartItem.product_id equals product.product_id
                        where customer.customer_id == customer_id
                        select new CartWithProducts
                        {
                            cart = cart,
                            cartItems = cartItem,
                            Product = product
                        };

            var cartss = await query.ToListAsync();

            return cartss;
        }


        public async Task<IActionResult> Cart()
        {
           
            var cartItems = await _context.CartItems.ToListAsync();

          
            int totalSubtotal = cartItems.Sum(ci => ci.subtotal);

    
            ViewBag.TotalSubtotal = totalSubtotal;



            return View(await GetCartAsync());


        }
        public async Task<IActionResult> Orders()
        {
            var customer_id = HttpContext.Session.GetInt32("id");
            return View( await _context.Orders.Where(m=>m.customer_id == customer_id).ToListAsync() );
        }
            
        public async Task<IActionResult> Store() => View(await _context.Product.Where(m=>m.product_stocks != 0).ToListAsync());




        public IActionResult AddCart(int quantity, int id, int price)
        {
            var customer_id = HttpContext.Session.GetInt32("id");
        
            
            Cart cart = new Cart { customer_id = customer_id };
            _context.Cart.Add(cart);
            _context.SaveChanges();

          

            var customer_cart = _context.Cart.FirstOrDefault(m => m.customer_id == customer_id);

            CartItems cartitems = new CartItems { cart_id = customer_cart.cart_id , product_id = id, quantity = quantity, subtotal = (price*quantity)};
            _context.CartItems.Add(cartitems);
            _context.SaveChanges();



            TempData["successcart"] = "Add Cart Successful";

            return RedirectToAction("Store");


        }

        public async Task<IActionResult> Checkout( int[] selectedItems)
        {
            
            var item = await GetCartAsync();
            var customer_id = HttpContext.Session.GetInt32("id");

            var total = 0;
           
           
            _context.SaveChanges();

            if (selectedItems != null && selectedItems.Length > 0)
            {
                for (int i = 0; i < selectedItems.Count(); i++)
                {
                    var cartItems = item.Where(ci => ci.cartItems.cart_item_id == selectedItems[i]).FirstOrDefault();
                    total += cartItems.cartItems.subtotal;
                }
                Orders order = new Orders { customer_id = customer_id, order_date = Convert.ToString(DateTime.Now), order_status = "Ordered", order_total = total  };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                for (int i = 0; i < selectedItems.Count(); i++)
                {
                    
                    var cartItems = item.Where(ci => ci.cartItems.cart_item_id == selectedItems[i]).FirstOrDefault();
                    var product = _context.Product.FirstOrDefault(m => m.product_id == cartItems.Product.product_id);
                    product.product_stocks -= cartItems.cartItems.quantity;
                    _context.Product.Update(product);
                    OrderDetails orderDetails = new OrderDetails { order_id = order.order_id, product_id = cartItems.Product.product_id, quantity = cartItems.cartItems.quantity, price = cartItems.cartItems.subtotal };
                    _context.OrderDetails.Add(orderDetails);
                    _context.CartItems.Remove(cartItems.cartItems);
                    await _context.SaveChangesAsync();
                }
                Records record = new Records
                {
                    customer_name = HttpContext.Session.GetString("name"),
                    order_date = order.order_date,
                    order_status = order.order_status,
                    order_total = order.order_total
                };
                _context.Records.Add(record);

                await _context.SaveChangesAsync();
            }
          
          

      

            TempData["successcart"] = "Checkout Successfully";
            return RedirectToAction("Cart");
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var customer_id = HttpContext.Session.GetInt32("id");
            var query = from customer in _context.Customer
                        join order in _context.Orders on customer.customer_id equals order.customer_id
                        join orderDetails in _context.OrderDetails on order.order_id equals orderDetails.order_id
                        join product in _context.Product on orderDetails.product_id equals product.product_id
                        where order.order_id == id
                        select new OrderWithProducts
                        {
                            orders = order,
                            customer = customer,
                            product = product,
                            details = orderDetails
                        };

            var orderWithProducts = await query.ToListAsync();
            var viewtotal =  _context.Orders.FirstOrDefault(m=>m.customer_id == customer_id && m.order_id == id);


            ViewBag.TotalSubtotal = viewtotal.order_total;
            return View(orderWithProducts);
        }
         
        public async Task<IActionResult> Remove(int id)
        {
            var cart_item = await _context.CartItems.FirstOrDefaultAsync(m=>m.cart_item_id == id);

            if(cart_item != null)
            {
                _context.CartItems.Remove(cart_item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Cart");
            
        }
        
            public async Task<IActionResult> Received(int id)
        {
            var order = await _context.Orders.Where(m => m.order_id == id).FirstOrDefaultAsync();
            if (order != null)
            {
                order.order_status = "Received";

                await _context.SaveChangesAsync();

                TempData["received"] = "The order has been successfully received!";
            }

            return RedirectToAction("Orders");
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var orderdetails = await _context.OrderDetails.Where(m => m.order_id == id).ToListAsync();

            foreach(var item in orderdetails)
            {
                var products =  _context.Product.Where(m => m.product_id == item.product_id).FirstOrDefault();
                products.product_stocks += item.quantity;
                _context.Product.Update(products);
            }
            var order = await _context.Orders.Where(m => m.order_id == id).FirstOrDefaultAsync();
            if (order != null)
            {
                order.order_status = "Cancelled";

                await _context.SaveChangesAsync();

                TempData["received"] = "The order has been successfully cancelled!";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Orders");
        }
    }
}
