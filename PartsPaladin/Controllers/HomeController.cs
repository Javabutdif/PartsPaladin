using Microsoft.AspNetCore.Mvc;
using PartsPaladin.Data;
using PartsPaladin.Models;
using System.Diagnostics;

namespace PartsPaladin.Controllers
{
    public class HomeController(ILogger<HomeController> logger,PartsPaladinContext dbcontext) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly PartsPaladinContext _context = dbcontext;


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult RegisterButton(string name, string email, string address, string city, string password, string conpassword)
        {
            Customer customer = new Customer { customer_name = name, customer_email = email, customer_address = address, customer_city = city, customer_password = password };
            _context.Customer.Add(customer);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful. You can now login with your credentials.";

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> LoginButton(string? email, string password)
        {
            if (email.CompareTo("admin@gmail.com") == 0 && password.CompareTo("admin") == 0)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                var customer =  _context.Customer.FirstOrDefault(m => m.customer_email == email && m.customer_password == password);
                if (customer == null)
                {
                    TempData["invalidLogin"] = "Incorrect email and password.";

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    HttpContext.Session.SetString("name", customer.customer_name);
                    HttpContext.Session.SetInt32("id", (int)customer.customer_id);
                    return RedirectToAction("Index", "User");
                }

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
