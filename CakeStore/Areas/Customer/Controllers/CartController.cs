using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.Models;
using Store.Models.ViewModels;
using Store.Utility;
using System.Security.Claims;

namespace CakeStore.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        [BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CartController(ApplicationDbContext db, IEmailSender emailSender)
		{
			_db = db;
			_emailSender = emailSender;
		}
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM = new()
			{
				ShoppingCartList = _db.ShoppingCarts.Where(u => u.ApplicationUserId == userId).Include("Product").ToList(),
				OrderHeader = new()
			};
            IEnumerable<ProductImage> productImages = _db.ProductImages.ToList();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
                cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
                cart.Price = cart.Product.Price * cart.Count;
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Product.Price * cart.Count;
			}
			return View(ShoppingCartVM);
		}
		public IActionResult Plus(int cartId)
		{
			var cartFromDb = _db.ShoppingCarts.Find(cartId);
			cartFromDb.Count += 1;
			_db.ShoppingCarts.Update(cartFromDb);
			_db.SaveChanges();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId)
		{
			var cartFromDb = _db.ShoppingCarts.Find(cartId);
			if (cartFromDb.Count <= 1)
			{
				//remove that from cart

				_db.ShoppingCarts.Remove(cartFromDb);
				HttpContext.Session.SetInt32(SD.SessionCart, _db.ShoppingCarts
					.Where(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
			}
			else
			{
				cartFromDb.Count -= 1;
				_db.ShoppingCarts.Update(cartFromDb);
			}

			_db.SaveChanges();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var cartFromDb = _db.ShoppingCarts.Find(cartId);

			_db.ShoppingCarts.Remove(cartFromDb);

			HttpContext.Session.SetInt32(SD.SessionCart, _db.ShoppingCarts
			  .Where(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
			_db.SaveChanges();
			return RedirectToAction(nameof(Index));
		}
		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM = new()
			{
				ShoppingCartList = _db.ShoppingCarts.Where(u => u.ApplicationUserId == userId).Include("Product").ToList(),
				OrderHeader = new()
			};
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				ShoppingCartVM.OrderHeader.OrderTotal += cart.Product.Price * cart.Count;
			}
			ShoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.Find(userId);

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
			return View(ShoppingCartVM);
		}

		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM.ShoppingCartList = _db.ShoppingCarts.Where(u => u.ApplicationUserId == userId).Include("Product").ToList();

			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			ApplicationUser applicationUser = _db.ApplicationUsers.Find(userId);


			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = cart.Product.Price;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				//it is a regular customer 
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			}
			else
			{
				//it is a company user
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			}
			_db.OrderHeaders.Add(ShoppingCartVM.OrderHeader);
			_db.SaveChanges();
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count
				};
				_db.OrderDetails.Add(orderDetail);
				_db.SaveChanges();
			}
			return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
		}
		public IActionResult OrderConfirmation(int id)
		{
            OrderHeader orderHeader = _db.OrderHeaders.Include("ApplicationUser").FirstOrDefault(u=>u.Id == id);
            List<ShoppingCart> shoppingCarts = _db.ShoppingCarts
               .Where(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _db.ShoppingCarts.RemoveRange(shoppingCarts);
            _db.SaveChanges();
			HttpContext.Session.Clear();
			if(orderHeader.ApplicationUser.Email != null)
			{
				string email = orderHeader.ApplicationUser.Email;
                _emailSender.SendEmailAsync(email, "New Order - Cake Store",
               $"<p>New Order Created - {orderHeader.Id}</p>");
            }
           
            return View(id);
		}
	}
}
