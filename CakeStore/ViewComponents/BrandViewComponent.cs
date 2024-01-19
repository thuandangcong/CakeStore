using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Data;
using Store.Models;

namespace CakeStore.ViewComponents
{
    public class BrandViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public BrandViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (HttpContext.Request.Query.TryGetValue("brandId", out var brandIdValue))
            {
                // brandIdValue là giá trị của tham số brandId, có thể chuyển đổi thành kiểu int
                if (int.TryParse(brandIdValue, out var brandId))
                {
                    // Bây giờ bạn có giá trị brandId để sử dụng
                    // brandId có thể được sử dụng trong logic của bạn ở đây
                    ViewBag.BrandId = brandId;
                }
            }

            List<Brand> brandList = _db.Brands.ToList();

            return View(brandList);
        }
    }
}
