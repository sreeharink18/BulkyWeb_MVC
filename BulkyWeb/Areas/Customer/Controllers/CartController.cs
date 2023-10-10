using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BulkyWeb.Utility;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        private IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork) { 
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                shoppingCartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, includeProperties: "Product"),
                OrderHeader = new()

            };
            
            foreach(var cart in ShoppingCartVM.shoppingCartsList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                shoppingCartsList = _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId==UserId ,includeProperties:"Product"),
                OrderHeader = new() 
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostCode;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            //Insert total amount
            foreach (var cart in ShoppingCartVM.shoppingCartsList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);  



        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
	        var UserIdentity = (ClaimsIdentity)User.Identity;
	        var UserId = UserIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

	        ShoppingCartVM.shoppingCartsList =
		        _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = UserId;
            

	        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);

	        /*ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
	        ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
	        ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
	        ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
	        ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
	        ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostCode;*/

	         foreach (var cart in ShoppingCartVM.shoppingCartsList)
	        {
		        cart.Price = GetPriceBasedOnQuantity(cart);
		        ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

	        }

	        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
	        {
                //it is an regular user
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.PaymentStatusPending;

	        }
	        else
	        {
				// it is company user
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.PaymentStatusApproved;

			}
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var cart in ShoppingCartVM.shoppingCartsList)
            {
                OrderDetail orderDetail = new()
                {
                    Count = cart.Count,
                    Price = cart.Price,
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id
                };
                _unitOfWork.OrderDetail.Add(orderDetail);   
                _unitOfWork.Save();
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //stripe implementation regular customer   
            }

			return RedirectToAction(nameof(OrderConfirmation),new {ShoppingCartVM.OrderHeader.Id});

        }

        public IActionResult OrderConfirmation(int id)
        {
	        return View(id);
        }
        public IActionResult Plus(int CartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == CartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int CartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == CartId);
            if(cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int CartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == CartId);           
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if (shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
    }
}
