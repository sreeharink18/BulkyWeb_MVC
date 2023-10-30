using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "category").ToList();
            return View(products);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "category"),
                Count = 1,
                ProductId = productId,

            };

            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = UserId;
            ShoppingCart cartFromdb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == UserId && u.ProductId == shoppingCart.ProductId);
            if (cartFromdb != null)
            {

                //If exists than Update
                cartFromdb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromdb);
                _unitOfWork.Save();
            }
            else
            {
                //Add new 
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.sessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId).Count());
                
            }
            
            TempData["success"] = "Shopping Cart is Updated";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
           

            return View();
            
        }
        [Authorize]
        public IActionResult UserProfile()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser Userobj = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);
            if (Userobj != null)
            {
                return View(Userobj);
            }
            return View();
        }
        public IActionResult EditUserProfile()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser Userobj = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);
            if (Userobj != null)
            {
                return View(Userobj);
            }
            return View();
        }
        [HttpPost]
        public IActionResult EditUserProfile(ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();
                return RedirectToAction(nameof(UserProfile));
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
