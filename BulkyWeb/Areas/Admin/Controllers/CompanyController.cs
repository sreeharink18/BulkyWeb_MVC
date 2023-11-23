using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using BulkyWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork) { 
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();
            return View(companies);
        }
        public IActionResult Upsert(int? id)
        {
            /*IEnumerable<SelectListItem> CompanyList = _unitOfWork.Company.GetAll()
                .Select(u => new SelectListItem { 
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });*/
            /*  ViewBag.CompanyList = CompanyList;
              ViewData["CompanyList"] = CompanyList;*/
           
            if (id == null || id == 0)
            {
                //Create
                return View(new Company());
            }
            else
            {
                //Update
                Company Companyobj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(Companyobj);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company Companyobj)
        {

            if (ModelState.IsValid)
            {
                
                if (Companyobj.Id == 0)
                {
                    _unitOfWork.Company.Add(Companyobj);
                    TempData["success"] = "Company Created Successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(Companyobj);
                    TempData["success"] = "Company Updated Successfully";
                }

                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            else
            {
               
                return View(Companyobj);
            }


        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Company = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = Company });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var Companyfromdb = _unitOfWork.Company.Get(u => u.Id == id);
            if (Companyfromdb == null)
            {
                return Json(new { success = false, errrorMessage = "Delete is failed" });
            }
            
            _unitOfWork.Company.Remove(Companyfromdb);
            _unitOfWork.Save();
            return Json(new { success = true, errrorMessage = "Delete is succesfully" });
        }
        #endregion
    }

}
