using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.Models;
using System.Diagnostics;

namespace CakeStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _logger = logger;
            _db = db;
            _environment = environment;
        }

        public IActionResult Index(int? page, string? orderBy)
        {
            List<Product> products = _db.Products.Take(8).Include("ProductImages").ToList();




            return View(products);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
