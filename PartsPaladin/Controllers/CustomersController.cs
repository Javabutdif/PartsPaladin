using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PartsPaladin.Data;
using PartsPaladin.Models;

namespace PartsPaladin.Controllers
{
    public class CustomersController : Controller
    {
        private readonly PartsPaladinContext _context;

        public CustomersController(PartsPaladinContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customer.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.customer_id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCustomer(string name, string email, string address, string city, string password, string conpassword)
        {
            Customer customer = new Customer { customer_name = name, customer_email = email, customer_address = address, customer_city = city, customer_password = password };
            _context.Customer.Add(customer);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful. You can now login with your credentials.";

            return RedirectToAction("Index");
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, string email, string address, string city)
        {
         
            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Update the properties
            customer.customer_name = name;
            customer.customer_email = email;
            customer.customer_address = address;
            customer.customer_city = city;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                   
                        throw;
                   
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer); // Assuming you want to return the same view with validation errors
        }


        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.customer_id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch related entities
            var cartItems = await _context.Cart.Where(m => m.customer_id == id).ToListAsync();
            var orders = await _context.Orders.Where(m => m.customer_id == id).ToListAsync();
            var customer = await _context.Customer.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            // Delete related cart items
            foreach (var cartItem in cartItems)
            {
                _context.Cart.Remove(cartItem);
            }

            // Delete related order details and orders
            foreach (var order in orders)
            {
                var orderDetails = await _context.OrderDetails.Where(od => od.order_id == order.order_id).ToListAsync();
                foreach (var orderDetail in orderDetails)
                {
                    _context.OrderDetails.Remove(orderDetail);
                }
                _context.Orders.Remove(order);
            }

            // Delete the customer
            _context.Customer.Remove(customer);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
