using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PartsPaladin.Data;
using PartsPaladin.Models;
using static NuGet.Packaging.PackagingConstants;

namespace PartsPaladin.Controllers
{
    public class ProductsController : Controller
    {
        private readonly PartsPaladinContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProductsController(PartsPaladinContext context , IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.product_id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string product_name, string product_description, int product_price, int product_stocks, IFormFile? product_image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (product_image != null && product_image.Length > 0)
                    {

                        string uniqueFileName = product_name + "_" + product_image.FileName;


                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);


                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await product_image.CopyToAsync(fileStream);
                        }


                        Product product = new Product
                        {
                            product_name = product_name,
                            product_description = product_description,
                            product_price = product_price,
                            product_stocks = product_stocks,
                            product_image = "/uploads/" + uniqueFileName,

                        };


                        _context.Add(product);

                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {

                        ModelState.AddModelError("image", "Please select an image file.");
                    }
                
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", "An error occurred while saving the apartment information.");

                    return View();
                }
            }
            return View();

        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string name, string description,int price, int stocks )
        {
            var prod = await _context.Product.FindAsync(id);
            if (prod == null)
            {
                return NotFound();
            }

            // Update the properties
            prod.product_name = name;
            prod.product_description = description;
            prod.product_price = price;
            prod.product_stocks = stocks;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    throw;

                }
                return RedirectToAction(nameof(Index));
            }
            return View(prod);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.product_id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var cart_item = await _context.CartItems.Where(m => m.product_id == id).ToListAsync();
            var orderdetails = await _context.OrderDetails.Where(m => m.product_id == id).ToListAsync();
            var product = await _context.Product.FindAsync(id);

            foreach (var cartItem in cart_item)
            {
                _context.CartItems.Remove(cartItem);
            }
            foreach (var orderitem in orderdetails)
            {
                _context.OrderDetails.Remove(orderitem);
            }

            if (product != null)
            {
                _context.Product.Remove(product);
            }
            TempData["delete"] = "Delete Product Successfull!";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int? id)
        {
            return _context.Product.Any(e => e.product_id == id);
        }
    }
}
