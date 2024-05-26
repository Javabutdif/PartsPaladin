using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsPaladin.Data;
using PartsPaladin.Models;

namespace PartsPaladin.Controllers
{
    public class AdminController(PartsPaladinContext cd) : Controller
    {
        private readonly PartsPaladinContext _context = cd;
        public async Task<IActionResult> Index()
        {
            var items = await _context.Records.Where(m=>m.order_status == "Delivered" || m.order_status == "Received").ToListAsync();
            ViewBag.total = items.Sum(m => m.order_total);
            return View();
        }
        public async Task<IActionResult> Orders()
        {
            return View(await _context.Customer
                    .Join(
                        _context.Orders,
                        customer => customer.customer_id,
                        order => order.customer_id,
                        (customer, order) => new OrderWithProducts
                        {
                            customer = customer,
                            orders = order
                        })
                    .ToListAsync());
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
            var viewtotal = _context.Orders.FirstOrDefault(m=>m.order_id == id);


            ViewBag.TotalSubtotal = viewtotal.order_total;
            return View(orderWithProducts);
        }

        public async Task<IActionResult> Delivered(int id)
        {
            var order = await _context.Orders.Where(m=>m.order_id==id).FirstOrDefaultAsync();
            var customer = await _context.Customer.Where(m => m.customer_id == order.customer_id).FirstOrDefaultAsync();
            if (order != null)
            {
                order.order_status = "Delivered";
                Records record = new Records
                {
                    customer_name = customer.customer_name ,
                    order_date = order.order_date,
                    order_status = order.order_status,
                    order_total = order.order_total
                };
                _context.Records.Add(record);

                await _context.SaveChangesAsync();

                TempData["successcart"] = "Order Delivery has been successfully updated!";
            }

            return RedirectToAction("Orders");
        }
        public async Task<IActionResult> Cancelled(int id)
        {
            var orderdetails = await _context.OrderDetails.Where(m => m.order_id == id).ToListAsync();

            foreach (var item in orderdetails)
            {
                var products = _context.Product.Where(m => m.product_id == item.product_id).FirstOrDefault();
                products.product_stocks += item.quantity;
                _context.Product.Update(products);
            }
            var order = await _context.Orders.Where(m => m.order_id == id).FirstOrDefaultAsync();
            var customer = await _context.Customer.Where(m => m.customer_id == order.customer_id).FirstOrDefaultAsync();
            if (order != null)
            {
                order.order_status = "Cancelled";
                Records record = new Records
                {
                    customer_name = customer.customer_name,
                    order_date = order.order_date,
                    order_status = order.order_status,
                    order_total = order.order_total
                };
                _context.Records.Add(record);

                await _context.SaveChangesAsync();

                TempData["cancel"] = "The cancellation of the order has been successfully processed!";
            }

            return RedirectToAction("Orders");
        }
        public async Task<IActionResult> Transaction()
        {
            return View(await _context.Records.ToListAsync());
        }


    }
}
