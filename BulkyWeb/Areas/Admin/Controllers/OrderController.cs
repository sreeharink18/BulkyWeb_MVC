using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public OrderController(IUnitOfWork unitOfWork) { 
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}
		#region Api Call
		[HttpGet]
		public IActionResult Get()
		{
			List<OrderHeader> orders = _unitOfWork.OrderHeader.GetAll(includeProperties :"ApplicationUser").ToList();
			return Json(new {data= orders });
			
		}
		#endregion
	}

}
