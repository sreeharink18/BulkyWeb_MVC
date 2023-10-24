using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BulkyWeb.Utility;
using Stripe.Checkout;

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
        public IActionResult setAddress(int? id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                shoppingCartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, includeProperties: "Product"),
                OrderHeader = new()
            };
            if(id == null)
            {
                ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);

                ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
                ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
                ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
                ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostCode;
                ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
                ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            }
            else
            {
                ShoppingCartVM.OrderHeader.MultipleAddress = _unitOfWork.AddMultipleAddressess.Get(u => u.Id == id);

                ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.MultipleAddress.Name;
                ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.MultipleAddress.State;
                ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.MultipleAddress.StreetAddress;
                ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.MultipleAddress.PostalCode;
                ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.MultipleAddress.City;
                ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.MultipleAddress.PhoneNumber;
            }
           

            //Insert total amount
            foreach (var cart in ShoppingCartVM.shoppingCartsList)
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
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;

	        }
	        else
	        {
				// it is company user
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;

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
				var domain = "https://localhost:7279/";
				var options = new SessionCreateOptions
				{
					SuccessUrl = domain+ $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain+"customer/cart/Index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

				foreach (var item in ShoppingCartVM.shoppingCartsList)
				{
					var sessionLineItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions()
						{
							UnitAmount = (long)(item.Price*100), //its can include in here about amount
							Currency = "INR",
							ProductData = new SessionLineItemPriceDataProductDataOptions()
							{
								Name = item.Product.Title
							}

						},
						Quantity = item.Count

					};  
                    options.LineItems.Add(sessionLineItem);

				}
				var service = new SessionService();
				Session session= service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id,session.Id,session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location",session.Url);
                return new StatusCodeResult(303);

            }

			return RedirectToAction(nameof(OrderConfirmation),new {ShoppingCartVM.OrderHeader.Id});

        }

        public IActionResult OrderConfirmation(int id)
        {
	        OrderHeader orderHeader =
		        _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
	        if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
	        {
		        var services = new SessionService();
		        Session session = services.Get(orderHeader.SessionId);
		        if (session.PaymentStatus.ToLower() == "paid")
		        {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(id,session.Id,session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id,SD.StatusApproved,SD.PaymentStatusApproved);
                    _unitOfWork.Save();
		        }
	        }

	        List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
		        .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
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
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == CartId, tracked: true);
            if(cartFromDb.Count <= 1)
            {
                HttpContext.Session.SetInt32(SD.sessionCart,_unitOfWork.ShoppingCart.GetAll(u=> u.ApplicationUserId== cartFromDb.ApplicationUserId).Count()-1);
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
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == CartId,tracked:true);
            HttpContext.Session.SetInt32(SD.sessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
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
