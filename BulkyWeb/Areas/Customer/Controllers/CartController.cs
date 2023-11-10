using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BulkyWeb.Utility;
using Stripe.Checkout;
using Stripe;
using Azure;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
       
		
		[BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
	

		private IUnitOfWork _unitOfWork;
        public static bool WalletChecked { get; set; }
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
      /*  [HttpPost]
        public IActionResult Index(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                shoppingCartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, includeProperties: "Product"),
                OrderHeader = new(),
                CouponList=_unitOfWork.Coupon.GetAll().ToList(),

            };

            foreach (var cart in ShoppingCartVM.shoppingCartsList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
			couponcode = _unitOfWork.Coupon.Get(u=>u.CouponCode == shoppingCartVM.OrderHeader.CouponCode);
            if(couponcode != null)
            {
                flag = 1;
				Ordertotal = (int)ShoppingCartVM.OrderHeader.OrderTotal;
				if (Ordertotal >= couponcode.MinAmout)
                {
					Ordertotal = Ordertotal - couponcode.DiscountAmout;
                }
                ShoppingCartVM.OrderHeader.OrderTotal = Ordertotal;
                ShoppingCartVM.OrderHeader.CouponCode = couponcode.CouponCode;
                *//*_unitOfWork.OrderHeader.Update(shoppingCartVM.OrderHeader);
                _unitOfWork.Save();
*//*
            }
            
            return View(ShoppingCartVM);
        }*/
       
		public IActionResult CheckOutCoupon(ShoppingCartVM shoppingCartVM)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM = new()
			{
				shoppingCartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, includeProperties: "Product"),
				OrderHeader = new(),
				CouponList = _unitOfWork.Coupon.GetAll().ToList(),

			};

			foreach (var cart in ShoppingCartVM.shoppingCartsList)
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
                OrderHeader = new(),
                ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId)
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

           
				foreach (var cart in ShoppingCartVM.shoppingCartsList)
				{
					cart.Price = GetPriceBasedOnQuantity(cart);
					ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
				}
          
            //Insert total amount

            return View(ShoppingCartVM);
        }
    /*    public IActionResult Summary()
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
            *//*foreach (var cart in ShoppingCartVM.shoppingCartsList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }*//*
            return View(ShoppingCartVM);  
        }*/

        [HttpPost]
        [ActionName("setAddress")]
        public IActionResult SummaryPost()
        {
	        var UserIdentity = (ClaimsIdentity)User.Identity;
	        var UserId = UserIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

	        ShoppingCartVM.shoppingCartsList =
		        _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, includeProperties: "Product");
			/* var methodPayment = ShoppingCartVM.OrderHeader.PaymentMethod;
			 Console.WriteLine("paymentMethod ......"+methodPayment.ToString());*/
			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = UserId;

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);
          	foreach (var cart in ShoppingCartVM.shoppingCartsList)
				{
					cart.Price = GetPriceBasedOnQuantity(cart);
					ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

				}
            foreach (var cart in ShoppingCartVM.shoppingCartsList)
            {
                if (cart.Product != null)
                {
                    // Decrement the TotalBookCount based on the cart's Count
                    if (cart.Product.TotalBooktCount.HasValue)
                    {
                        cart.Product.TotalBooktCount -= cart.Count;

                        // Ensure the TotalBookCount doesn't go below 0
                        if (cart.Product.TotalBooktCount < 0)
                        {
                            cart.Product.TotalBooktCount = 0;
                        }
                    }
                }
            }
            //Its is COD 
            if (ShoppingCartVM.OrderHeader.PaymentMethod == SD.PaymentMethodCOD.ToString())
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentMethodCODPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;          
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
            }//Online Payment
            else if(ShoppingCartVM.OrderHeader.PaymentMethod == SD.PaymentMethodOnline.ToString())
            {
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
                    decimal totalAmount=(decimal)ShoppingCartVM.OrderHeader.OrderTotal;

					if (!string.IsNullOrEmpty(ShoppingCartVM.OrderHeader.CouponCode))
                    {
						var coupon = _unitOfWork.Coupon.Get(u => u.CouponCode == ShoppingCartVM.OrderHeader.CouponCode);
                        totalAmount = CouponCheckOut(ShoppingCartVM.OrderHeader.CouponCode, (int)ShoppingCartVM.OrderHeader.OrderTotal);
                        if(coupon != null)
                        {
                            ShoppingCartVM.OrderHeader.OrderTotal = (double)totalAmount;
                        }
					}
                    if (WalletChecked)
                    {
                        totalAmount = 40;
                        var userobj = _unitOfWork.ApplicationUser.Get(u => u.Id == applicationUser.Id);
                        int walletAmout = (int)userobj.Wallet;
                        int totalAmountOrderHeadear = (int)ShoppingCartVM.OrderHeader.OrderTotal-40;

						if (walletAmout > 0)
                        {
                            walletAmout = walletAmout - totalAmountOrderHeadear;
                            userobj.Wallet = walletAmout;
                            _unitOfWork.ApplicationUser.Update(userobj);
                            _unitOfWork.Save();
                        }
                    }
                    //stripe implementation regular customer   
                    var domain = "https://localhost:7279/";
					var options = new SessionCreateOptions
					{
						SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
						CancelUrl = domain + "customer/cart/Index",
						LineItems = new List<SessionLineItemOptions>(),
						Mode = "payment",
					};
					

					// Create a payment intent
					

					
						var sessionLineItem = new SessionLineItemOptions
						{
							PriceData = new SessionLineItemPriceDataOptions()
							{
								UnitAmount = (long?)(totalAmount * 100), //its can include in here about amount
								Currency = "INR",
                                
								ProductData = new SessionLineItemPriceDataProductDataOptions()
								{
                                    /*foreach(var item  in ShoppingCartVM.shoppingCartsList)
                                    {
						            Name = item.Product.Title,

									}*/
					                Name = "Book Store",
								}

							},
							Quantity = 1,

                        };
                        


                        options.LineItems.Add(sessionLineItem);


					//foreach (var item in ShoppingCartVM.ShoppingCartList)
					//{
					//    var sessionLineItem = new SessionLineItemOptions
					//    {
					//        PriceData = new SessionLineItemPriceDataOptions
					//        {
					//            UnitAmount = (long)(item.Price * 100),
					//            Currency = "inr",
					//            ProductData = new SessionLineItemPriceDataProductDataOptions
					//            {
					//                Name = item.Product.Title
					//            }
					//        },
					//        Quantity = item.Count
					//    };
					//    options.LineItems.Add(sessionLineItem);
					//}

					var service = new SessionService();
					Session session = service.Create(options);
					_unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
					_unitOfWork.Save();
					Response.Headers.Add("Location", session.Url);
					return new StatusCodeResult(303);

				}
			}
			return RedirectToAction(nameof(OrderConfirmation),new {ShoppingCartVM.OrderHeader.Id});

        }

        public IActionResult OrderConfirmation(int id)
        {
	        OrderHeader orderHeader =
		        _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if(orderHeader.PaymentMethod==SD.PaymentMethodOnline)
            {
				if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
				{
					var services = new SessionService();
					Session session = services.Get(orderHeader.SessionId);
					if (session.PaymentStatus.ToLower() == "paid")
					{
						_unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
						_unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
						_unitOfWork.Save();
					}
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
        public async Task<IActionResult> Coupon(string coupon, int? OrderTotal)
        {
            if(string.IsNullOrEmpty(coupon) || OrderTotal== null)
            {
                return BadRequest();
            }
            var couponobj = _unitOfWork.Coupon.Get(u=>u.CouponCode == coupon);

            if(couponobj != null)
            {
                if(couponobj.DiscountAmout < OrderTotal)
                {
                    decimal discountPrice = (decimal)couponobj.DiscountAmout;
                    decimal cartTotal = (decimal)OrderTotal;
                    if(couponobj.DiscountAmout > 0)
                    {
                        discountPrice = cartTotal-couponobj.DiscountAmout;
                    }
                    else
                    {
						discountPrice = (decimal)(cartTotal - (cartTotal) * (couponobj.DiscountAmout / 100));
					}
                    decimal newTotal = (decimal)(OrderTotal - discountPrice);
                    
                    var responce = new {
                        success = true,
						discountPrice,
						newTotal
					};
                    return Json(responce);
                }
                else
                {
					TempData["error"] = "Order total is below the minimum purchase amount.";
                    var responce = new {
                        success= false,
                        errorMessage = "Order total is below the minimum purchase amount."
				
				    };
                    return Json(responce);
                }
            }
			TempData["error"] = "Coupon not found.";
			var responsed = new
			{
				success = false,
				errorMessage = "Coupon not found"
			};
			return Json(responsed);
		}
        private decimal CouponCheckOut(string couponCode,int orderTotal)
        {
            var couponobj = _unitOfWork.Coupon.Get(u=>u.CouponCode == couponCode);
            decimal newTotal=(decimal)orderTotal;
            if(couponobj != null)
            {
                if(couponobj.MinAmout < orderTotal)
                {
                    if(couponobj.DiscountAmout > 0)
                    {
                        newTotal = (decimal)(orderTotal - couponobj.DiscountAmout);
                    }
                    else
                    {
                        newTotal = (decimal)(orderTotal - (orderTotal) * (couponobj.DiscountAmout / 100));
					}
                }
            }
            return newTotal;
        }
        public IActionResult CheckWallet(int? totalAmount, string? userId)
        {
            var userobj = _unitOfWork.ApplicationUser.Get(u=>u.Id == userId);
   
			string message = "";

			if (userobj.Wallet > totalAmount - 40)
            {
                var newwalletAmount = userobj.Wallet - totalAmount - 40;
                 message = "If we pay a minimum amount of 40 in online payment";
				var response = new
				{
					success =true,
                    newWalletAmount = newwalletAmount,
					Message= message,

				};
				WalletChecked = true;
				return Json(response);

            }
            else
            {
				var response = new
				{
					success = false,
					newWalletAmount = (int)totalAmount,
					Message = "The Wallet have enough Amount"

				};
				
				return Json(response);
			}
			
		}
		public IActionResult IsNotCheckWallet(int? totalAmount, string? userId)
		{
			var userobj = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			string message = "";

			if (userobj.Wallet > totalAmount - 40)
			{

				var newwalletAmount = userobj.Wallet - totalAmount - 40;
				message = "If we pay a minimum amount of 40 in online payment";
				var response = new
				{
					success = true,
				

				};
                WalletChecked = false;
				return Json(response);

			}
			else
			{
				var response = new
				{
					success = false,
					newWalletAmount = (int)totalAmount,
					Message = "The Wallet have enough Amount"

				};
				return Json(response);
			}

		}

	}
}
