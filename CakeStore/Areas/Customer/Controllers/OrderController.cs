using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.Models;
using Store.Models.ViewModels;
using System.Security.Claims;

namespace CakeStore.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

           List<OrderHeader> objOrderHeaders = _db.OrderHeaders.Where(u => u.ApplicationUserId == userId).Include("ApplicationUser").ToList();
            return View(objOrderHeaders);
        }
        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = _db.OrderHeaders.Include("ApplicationUser").FirstOrDefault(u => u.Id == orderId),
                OrderDetail = _db.OrderDetails.Where(u => u.OrderHeaderId == orderId).Include("Product")
            };


            return View(OrderVM);
        }
    }

}
