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
            List<Coupon> coupon = _unitOfWork.Coupon.GetAll().ToList();

            return View(coupon);
        }
        public IActionResult AddCoupon()
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
        public IActionResult Edit(int id)
        {
           Coupon coupon = _unitOfWork.Coupon.Get(u=>u.Id == id);
            return View(coupon);
        }
        [HttpPost]
        public IActionResult Edit(Coupon coupon)
        {
           if (coupon != null)
            {
                _unitOfWork.Coupon.Update(coupon);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }
        public IActionResult Delete(int id)
        {
            Coupon coupon = _unitOfWork.Coupon.Get(u=>u.Id == id);
            return View(coupon);
        }
        [HttpPost]
        public IActionResult Delete(Coupon coupon)
        {
            if (coupon == null)
            {
                NotFound();
            }
            _unitOfWork.Coupon.Remove(coupon);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        
            
    }
}
