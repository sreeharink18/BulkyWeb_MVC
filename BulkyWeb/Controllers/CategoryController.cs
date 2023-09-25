
using BulkyWeb.DataAccess.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        public readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db) { 
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> category = _db.categories.ToList();
            return View(category);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj) {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The display order cannot exactly same");
            }
            if (ModelState.IsValid)
            {
                _db.categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
           
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? CategoryFromDb1 = _db.categories.FirstOrDefault(u => u.Id == id);
            /*Category? CategoryFromDb = _db.Categories.Find(id);*//*
            Category? CategoryFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();*/
            if (CategoryFromDb1 == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb1);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {

            if (ModelState.IsValid)
            {
               _db.categories.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Category Edit Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? CategoryFromDb1 = _db.categories.FirstOrDefault(u => u.Id == id);
            /*Category? CategoryFromDb = _db.Categories.Find(id);*//*
            Category? CategoryFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();*/
            if (CategoryFromDb1 == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb1);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? obj = _db.categories.FirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            _db.categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Category delete Successfully";
            return RedirectToAction("Index");

        }

    }
}
