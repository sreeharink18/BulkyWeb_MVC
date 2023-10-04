using BulkyWeb.DataAccess.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using BulkyWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        /* private readonly IUnitOfWork _unitOfWork;*/
        /*public UserController(IUnitOfWork db) {
            _unitOfWork = db;
        }*/
        public UserController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public IActionResult Index()
        {
            List<ApplicationUser> userfromdb = _context.ApplicationUsers.Include(u=>u.company).ToList();

            return View(userfromdb);
        }
       
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userfromdb = _context.ApplicationUsers.Include(u => u.company).ToList();
            return Json(new { data = userfromdb });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            return Json(new { success = true, errrorMessage = "Delete is succesfully" });
        }
        #endregion
    }

}
