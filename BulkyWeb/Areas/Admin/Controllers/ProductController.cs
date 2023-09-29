using BulkyWeb.DataAccess.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IActionResult Upsert(int? id)
        {
            /*IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem { 
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });*/
            /*  ViewBag.CategoryList = CategoryList;
              ViewData["CategoryList"] = CategoryList;*/
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),
            Product =new Product()
            };
            if (id == null || id == 0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //Update
                productVM.Product = _unitOfWork.Product.Get(u=>u.Id  == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVm ,IFormFile? form)
        {
         
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(productVm.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVm.CategoryList = _unitOfWork.Category.GetAll()
               .Select(u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString(),
               });
               return View(productVm);
            }

            
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? ProductFromDb1 = _unitOfWork.Product.Get(u => u.Id == id);
            /*Product? ProductFromDb = _db.Categories.Find(id);*//*
            Product? ProductFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();*/
            if (ProductFromDb1 == null)
            {
                return NotFound();
            }
            return View(ProductFromDb1);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product Edit Successfully";
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
            Product? ProductFromDb1 = _unitOfWork.Product.Get(u => u.Id == id);
            /*Product? ProductFromDb = _db.Categories.Find(id);*//*
            Product? ProductFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();*/
            if (ProductFromDb1 == null)
            {
                return NotFound();
            }
            return View(ProductFromDb1);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product delete Successfully";
            return RedirectToAction("Index");

        }

    }
}
