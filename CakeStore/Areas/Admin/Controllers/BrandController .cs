using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Data;
using Store.Models;
using Store.Utility;

namespace CakeStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BrandController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Brand> objBrandList = _db.Brands.ToList();
            ViewBag.TableName = "Brand Table";
            return View(objBrandList);
        }
        public IActionResult Create()
        {
            ViewBag.TableName = "Create Brand";
            return View();
        }
        [HttpPost]
        public IActionResult Create(Brand obj)
        {
            if (ModelState.IsValid)
            {
                _db.Brands.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Brand created successfully";
                return RedirectToAction("Index");

            }
            ViewBag.TableName = "Create Brand";
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Brand? categoryFromDb = _db.Brands.Find(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            ViewBag.TableName = "Edit Brand";
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Brand obj)
        {
            if (ModelState.IsValid)
            {
                _db.Brands.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Brand updated successfully";
                return RedirectToAction("Index");

            }
            ViewBag.TableName = "Edit Brand";
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Brand? categoryFromDb = _db.Brands.Find(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            ViewBag.TableName = "Delete Brand";
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Brand obj = _db.Brands.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Brands.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Brand deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
