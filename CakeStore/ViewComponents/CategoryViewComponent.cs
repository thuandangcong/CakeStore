using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Data;
using Store.Models;

namespace CakeStore.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public CategoryViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (HttpContext.Request.Query.TryGetValue("categoryId", out var categoryIdValue))
            {
                // categoryIdValue là giá trị của tham số categoryId, có thể chuyển đổi thành kiểu int
                if (int.TryParse(categoryIdValue, out var categoryId))
                {
                    // Bây giờ bạn có giá trị categoryId để sử dụng
                    // categoryId có thể được sử dụng trong logic của bạn ở đây
                    ViewBag.CategoryId = categoryId;
                }
            }

            List<Category> categoryList = _db.Categories.ToList();
           
            return View(categoryList);
        }
    }
}
