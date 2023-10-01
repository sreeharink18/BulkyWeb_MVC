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
        public readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        
        {
            List<Product> Product = _unitOfWork.Product.GetAll(includeProperties:"category").ToList();
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
        public IActionResult Upsert(ProductVM productVm ,IFormFile? file)
        {
         
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if(!string.IsNullOrEmpty(productVm.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath =
                            Path.Combine(wwwRootPath, productVm.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVm.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if(productVm.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVm.Product);
                    TempData["success"] = "Product Created Successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVm.Product);
                    TempData["success"] = "Product Updated Successfully";
                }
                
                _unitOfWork.Save();
               
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
        /*public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? ProductFromDb1 = _unitOfWork.Product.Get(u => u.Id == id);
            *//*Product? ProductFromDb = _db.Categories.Find(id);*//*
            Product? ProductFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();*//*
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

        }*/
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() {
            List<Product> Product = _unitOfWork.Product.GetAll(includeProperties: "category").ToList();
            return Json(new {data = Product}); 
        }
        [HttpDelete] 
        public IActionResult Delete(int? id) { 
            var productfromdb = _unitOfWork.Product.Get(u=>u.Id == id);
            if(productfromdb == null)
            {
                return Json(new {success = false , errrorMessage = "Delete is failed"});
            }
            var oldImagePath =
                           Path.Combine(_webHostEnvironment.WebRootPath, productfromdb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productfromdb);
            _unitOfWork.Save();
            return Json(new { success = true, errrorMessage = "Delete is succesfully" });
        }
        #endregion

    }
}
