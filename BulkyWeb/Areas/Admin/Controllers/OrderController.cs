using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using BulkyWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        private OrderVM OrderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork) { 
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}
        public IActionResult Details(int id)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties : "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == id, includeProperties:"Product")
            };
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
        #region Api Call
        [HttpGet]
		public IActionResult Get(string status)
		{
			IEnumerable<OrderHeader> orders = _unitOfWork.OrderHeader.GetAll(includeProperties :"ApplicationUser").ToList();
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
