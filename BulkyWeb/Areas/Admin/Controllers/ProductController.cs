using BulkyWeb.DataAccess.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> Product = _unitOfWork.Product.GetAll().ToList();
            return View(Product);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Product obj)
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
