using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerCouponController : Controller
    {
        public IUnitOfWork _unitOfWork;

        public CustomerCouponController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        public IActionResult Index()
        {
          
            List<Coupon> coupon = _unitOfWork.Coupon.GetAll().ToList();
            return View(coupon);
        }
    }
}
