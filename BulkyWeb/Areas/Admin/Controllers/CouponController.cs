using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        public IUnitOfWork _unitOfWork;
        [BindProperty]
        public CouponVM CouponVM { get; set; }
        public CouponController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            CouponVM = new() {
                ApplicationUser = _unitOfWork.ApplicationUser.GetAll().ToList(),
                Coupon =new() 
            };

            return View(CouponVM);
        }
        public IActionResult AddCoupon(string userid)
        {
            return View();
        }
        [HttpPost]
        
        public IActionResult AddCoupon(Coupon coupon)
        {
            if(coupon != null)
            {
                _unitOfWork.Coupon.Add(coupon);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
