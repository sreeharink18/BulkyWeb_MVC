using BulkyWeb.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index()
        {

            return View();
        }
    }
}
