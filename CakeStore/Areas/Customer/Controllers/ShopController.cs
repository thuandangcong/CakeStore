using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.Models;
using Store.Utility;
using System.Diagnostics;
using System.Security.Claims;
using X.PagedList;

namespace CakeStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShopController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;
        public ShopController(ILogger<HomeController> logger, ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _logger = logger;
            _db = db;
            _environment = environment;
        }

        public IActionResult Index(int? page, string? orderBy)
        {


            List<Product> productList = _db.Products.Include("Category").Include("ProductImages").ToList();


            if (!String.IsNullOrEmpty(orderBy))
            {
                productList = SortProduct.Sort(productList, orderBy);
                ViewBag.OrderBy = orderBy;
            }

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePageOfProducts = productList.ToPagedList(pageNumber, 9);


            return View(onePageOfProducts);
        }
        public IActionResult ByCategory(int categoryId, int? page, string? orderBy)
        {
            if (categoryId == null || categoryId == 0)
            {
                return NotFound();
            }
            ViewBag.CategoryId = categoryId;
            List<Product> productList = _db.Products.Where(u => u.CategoryId == categoryId).Include("ProductImages").ToList();
            if (!String.IsNullOrEmpty(orderBy))
            {
                productList = SortProduct.Sort(productList, orderBy);
                ViewBag.OrderBy = orderBy;
            }
            var pageNumber = page ?? 1;
            var onePageOfProducts = productList.ToPagedList(pageNumber, 9);
            return View(onePageOfProducts);
        }
        public IActionResult ByBrand(int brandId, int? page, string? orderBy)
        {
            if (brandId == null || brandId == 0)
            {
                return NotFound();
            }
            ViewBag.BrandId = brandId;
            List<Product> productList = _db.Products.Where(u => u.BrandId == brandId).Include("ProductImages").ToList();
            if (!String.IsNullOrEmpty(orderBy))
            {
                productList = SortProduct.Sort(productList, orderBy);
                ViewBag.OrderBy = orderBy;
            }
            var pageNumber = page ?? 1;
            var onePageOfProducts = productList.ToPagedList(pageNumber, 9);
            return View(onePageOfProducts);
        }
        [HttpPost]
        public IActionResult Search(String search, int? page, string? orderBy)
        {
            ViewBag.Search = search;
            List<Product> productList = _db.Products.Include("Category").Include("ProductImages").Where(u => u.Name.Contains(search)).ToList();
            if (!String.IsNullOrEmpty(orderBy))
            {
                productList = SortProduct.Sort(productList, orderBy);
                ViewBag.OrderBy = orderBy;
            }
            var pageNumber = page ?? 1;
            var onePageOfProducts = productList.ToPagedList(pageNumber, 9);
            return View(onePageOfProducts);
        }
        public IActionResult Details(int productId)
        {
            if (productId == null || productId == 0)
            {
                return NotFound();
            }

            Product? productFromDb = _db.Products.Include("Category").Include("ProductImages").FirstOrDefault(u => u.Id == productId);
            if (productFromDb == null)
            {
                return NotFound();
            }
            ShoppingCart cart = new()
            {
                Product = productFromDb,
                Count = 1,
                ProductId = productId
            };
            return View(cart);

        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart? cartFromDb = (ShoppingCart?)_db.ShoppingCarts.FirstOrDefault(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);
            if (cartFromDb != null)
            {
                cartFromDb.Count += shoppingCart.Count;
                _db.ShoppingCarts.Update(cartFromDb);
                _db.SaveChanges();
            }
            else
            {
                _db.ShoppingCarts.Add(shoppingCart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(SD.SessionCart,
                _db.ShoppingCarts.Where(u => u.ApplicationUserId == userId).Count());
            }

            TempData["success"] = "Cart update successfully";

            return RedirectToAction("Index");

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
