﻿using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using BulkyWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork) { 
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}
        public IActionResult Details(int id)
        {
            var orderHeaderobj = _unitOfWork.OrderHeader.Get(u => u.Id == id);
            double totalAmount=0;
            OrderVM = new()
            {
                
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties : "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == id, includeProperties:"Product"),
                Coupon = _unitOfWork.Coupon.Get(u => u.CouponCode == orderHeaderobj.CouponCode),
            };
            foreach(var item in OrderVM.OrderDetails) { 
                int count = item.Count;
                double price = item.Price;
                totalAmount += (count * price);
            }
            ViewBag.TotalAmount = totalAmount;
            return View(OrderVM);
        }
        [HttpPost]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFrom = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFrom.Name = OrderVM.OrderHeader.Name;
            orderHeaderFrom.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFrom.State = OrderVM.OrderHeader.State;
            orderHeaderFrom.City = OrderVM.OrderHeader.City;
            orderHeaderFrom.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeaderFrom.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFrom.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFrom.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFrom);
            _unitOfWork.Save();
            TempData["success"] = "Order Detail Update Succesfully";
            return RedirectToAction(nameof(Details),new {Id = orderHeaderFrom.Id});
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Inprocess Update Succesfully";
            return RedirectToAction(nameof(Details),new { Id = OrderVM.OrderHeader.Id});
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderFromdb = _unitOfWork.OrderHeader.Get(u=>u.Id ==  OrderVM.OrderHeader.Id);
            orderFromdb.Carrier = OrderVM.OrderHeader.Carrier;
            orderFromdb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderFromdb.OrderStatus = SD.StatusShipping;
            orderFromdb.ShippingDate = DateTime.Now;

            if(orderFromdb.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderFromdb.PaymentDueDate = (DateTime.Now.AddDays(30));
            }
            _unitOfWork.OrderHeader.Update(orderFromdb);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Shipped Update Succesfully";
            return RedirectToAction(nameof(Details), new { Id = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
       
        public IActionResult CancelOrder()
        {
            var orderFromdb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            if(orderFromdb.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions { 
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFromdb.PaymentIntentId
                };
                var services = new RefundService();
                Refund refund = services.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderFromdb.Id, SD.StatusCancelled, SD.StatusRefunded);
               
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderFromdb.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["success"] = "Order Status Cancelled Succesfully";
            return RedirectToAction(nameof(Details), new { Id = OrderVM.OrderHeader.Id });
        }
        [ActionName("Details")]
        [HttpPost]
        public IActionResult Detail_Pay_Now()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeader.Get(u=>u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetails = _unitOfWork.OrderDetail.GetAll(u=>u.OrderId == OrderVM.OrderHeader.Id , includeProperties: "Product");

            //stripe payment
            var domain = Request.Scheme+"://"+Request.Host.Value+"/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/Order/OrderConfirmation?OrderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?OrderId={OrderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderVM.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Price * 100), //its can include in here about amount
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
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303); 
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StatusDelivery()
        {
            var orderobj= _unitOfWork.OrderHeader.Get(u=>u.Id==OrderVM.OrderHeader.Id);
            orderobj.OrderStatus = SD.StatusDelivered;
            orderobj.DeliveryDate = DateTime.Now;
            _unitOfWork.OrderHeader.Update(orderobj);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Delivery Update Succesfully";
            return RedirectToAction(nameof(Details), new { Id = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        public IActionResult ReturnRequest()
        {
            var orderobj = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderobj.OrderStatus = SD.ReturnRequest;
            
            _unitOfWork.OrderHeader.Update(orderobj);
            _unitOfWork.Save();
            TempData["success"] = "Sent Return Order Request";
            return RedirectToAction(nameof(Details), new { Id = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        public IActionResult OrderReturnConfirmation()
        {
            var orderobj = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            var userobj = _unitOfWork.ApplicationUser.Get(u => u.Id == orderobj.ApplicationUserId);
            orderobj.OrderStatus = SD.StatusReturned;
            orderobj.PaymentStatus = SD.StatusRefunded;
            if(orderobj.PaymentMethod == SD.PaymentMethodOnline)
            {
                if(userobj.Wallet == null)
                {
                    userobj.Wallet = 0;
                }
                userobj.Wallet += (int)orderobj.OrderTotal;
            }
            orderobj.ReturnDate = DateTime.Now;
            _unitOfWork.OrderHeader.Update(orderobj);
            _unitOfWork.ApplicationUser.Update(userobj);
            _unitOfWork.Save();
            TempData["success"] = "Order Return  Succesfully";
            return RedirectToAction(nameof(Details), new { Id = OrderVM.OrderHeader.Id });
        }
        public IActionResult OrderConfirmation(int OrderHeaderId)
        {
            OrderHeader orderHeader =
                _unitOfWork.OrderHeader.Get(u => u.Id == OrderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var services = new SessionService();
                Session session = services.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(OrderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

            
            return View(OrderHeaderId);
        }
        public IActionResult Invoice(int orderId)
        {
            double orginaltotalAmount = 0;
            OrderHeader order = _unitOfWork.OrderHeader.Get(u=>u.Id == orderId);
            ApplicationUser user = _unitOfWork.ApplicationUser.Get(u => u.Id == order.ApplicationUserId);
            Models.Coupon coupon = _unitOfWork.Coupon.Get(u => u.CouponCode == order.CouponCode);
            List<OrderDetail> OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product").ToList();
            foreach (OrderDetail detail in OrderDetails)
            {
                orginaltotalAmount += (detail.Price * detail.Count);
            }

           
            OrderVM orderVM = new()
            {
                OrderHeader = order,
                User = user,
                Coupon = coupon,
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product"),
                OrginalTotalAmount = orginaltotalAmount,
            };
            return View(orderVM);
        }
        #region Api Call
        [HttpGet]
		public IActionResult Get(string status)
		{
			IEnumerable<OrderHeader> orders;
            if(User.IsInRole(SD.Role_Admin)|| User.IsInRole(SD.Role_Employee))
            {
                orders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            }
            else
            {
                var UserIdentity = (ClaimsIdentity)User.Identity;
                var UserId = UserIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                orders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == UserId, includeProperties: "ApplicationUser").ToList();
            }
            switch (status)
            {
                case "pending":
                    orders = orders.Where(u=>u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "completed":
                    orders = orders.Where(u => u.OrderStatus == SD.StatusShipping);
                    break;
                case "inprocess":
                    orders = orders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "approved":
                    orders = orders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                   
                    break;
            }
            return Json(new {data= orders });
			
		}
		#endregion
	}

}
