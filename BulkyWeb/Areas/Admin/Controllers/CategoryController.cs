using BulkyWeb.DataAccess.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        public static int categoryCount;
        public readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> category = _unitOfWork.Category.GetAll().ToList();
            categoryCount = category.Count();
            ViewBag.CategoryCount = categoryCount;
            return View(category);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The display order cannot exactly same");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
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
            Category? CategoryFromDb1 = _unitOfWork.Category.Get(u => u.Id == id);
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
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
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
            Category? CategoryFromDb1 = _unitOfWork.Category.Get(u => u.Id == id);
            /*Category? CategoryFromDb = _db.Categories.Find(id);*//*
            Category? CategoryFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();*/
            ViewBag.CategoryCount = categoryCount;
            if (CategoryFromDb1 == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb1);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category delete Successfully";
            return RedirectToAction("Index");

        }

    }
}
