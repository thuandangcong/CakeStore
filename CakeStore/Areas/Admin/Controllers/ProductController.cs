using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.Models;
using Store.Utility;

namespace CakeStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _db.Products.Include("Category").Include("Brand").Include("ProductImages").ToList();
            ViewBag.TableName = "Product Table";

            return View(objProductList);
        }
        #region Create
        public IActionResult Create()
        {
            ViewBag.CategoryList = new SelectList(_db.Categories.ToList(), "Id", "Name");
            ViewBag.BrandList = new SelectList(_db.Brands.ToList(), "Id", "Name");
            ViewBag.TableName = "Product Create";
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product obj, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                obj.CreatedDate = System.DateTime.Now;
                obj.UpdatedDate = System.DateTime.Now;
                _db.Products.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Product created successfully";


                string wwwRootPath = _environment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + obj.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);
                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = obj.Id,
                        };

                        if (obj.ProductImages == null)
                            obj.ProductImages = new List<ProductImage>();

                        obj.ProductImages.Add(productImage);
                    }

                }
                obj.UpdatedDate = System.DateTime.Now;
                _db.Products.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TableName = "Product Create";

            ViewBag.CategoryList = new SelectList(_db.Categories.ToList(), "Id", "Name");
            ViewBag.BrandList = new SelectList(_db.Brands.ToList(), "Id", "Name");
            return View();
        }
        #endregion
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            ViewBag.CategoryList = new SelectList(_db.Categories.ToList(), "Id", "Name");
            ViewBag.BrandList = new SelectList(_db.Brands.ToList(), "Id", "Name");
            Product? productFromDb = _db.Products.Include("ProductImages").FirstOrDefault(u => u.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            ViewBag.TableName = "Product Edit";

            return View(productFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Product obj, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _environment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + obj.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);
                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = obj.Id,
                        };

                        if (obj.ProductImages == null)
                            obj.ProductImages = new List<ProductImage>();

                        obj.ProductImages.Add(productImage);
                    }

                }
                obj.UpdatedDate = System.DateTime.Now;
                _db.Products.Update(obj);

                _db.SaveChanges();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");

            }
            ViewBag.CategoryList = new SelectList(_db.Categories.ToList(), "Id", "Name");
            ViewBag.BrandList = new SelectList(_db.Brands.ToList(), "Id", "Name");
            ViewBag.TableName = "Product Edit";

            return View();
        }
        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _db.ProductImages.Find(imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath =
                                   Path.Combine(_environment.WebRootPath,
                                   imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _db.ProductImages.Remove(imageToBeDeleted);
                _db.SaveChanges();

                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction(nameof(Edit), new { id = productId });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var productListWithImageUrl = _db.Products
      .Include(p => p.Category)
      .Include(p => p.Brand)
      .Include(p => p.ProductImages)
      .Select(p => new
      {
          id = p.Id,
          price = p.Price,
          name = p.Name,
          categoryName = p.Category.Name,
          brandName = p.Brand.Name,

          imageUrl = p.ProductImages.FirstOrDefault().ImageUrl
      })
      .ToList();

            return Json(new { data = productListWithImageUrl });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product? productToBeDeleted = _db.Products.Find(id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_environment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }
            _db.Products.Remove(productToBeDeleted);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
