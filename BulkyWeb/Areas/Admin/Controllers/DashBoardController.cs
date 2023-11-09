using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashBoardController : Controller
    {
        public IUnitOfWork _unitOfWork { get; set; }
        public DashBoardController(IUnitOfWork unitOfWork) { 
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            double totalAmount = GetTotalAmount();
            int OrderCount = GetOrderTotalCount();
            int ProductCount = GetProductCount();
            double MonthlyEarning = GetMonthlyEarning();
            List<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll().ToList();
            List<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll().ToList();
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            List<Product> products = _unitOfWork.Product.GetAll().ToList();
            DashboardVm dashboardVm =new()
            {
                OrderTotalAmount = totalAmount,
                ProductCount = ProductCount,
                OrdersCount = OrderCount,
                MonthlyRevenue = MonthlyEarning,
                Product = products,
                ApplicationUser = users,    
                OrderHeader = orderHeaders,
                Category = categories,
            };
            return View(dashboardVm);
        }
        public IActionResult Details()
        {
            double totalAmount = GetTotalAmount();
            int OrderCount = GetOrderTotalCount();
            int ProductCount = GetProductCount();
            double MonthlyEarning = GetMonthlyEarning();
            List<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll().ToList();
            List<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll().ToList();
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            List<Product> products = _unitOfWork.Product.GetAll().ToList();
            DashboardVm dashboardVm = new()
            {
                OrderTotalAmount = totalAmount,
                ProductCount = ProductCount,
                OrdersCount = OrderCount,
                MonthlyRevenue = MonthlyEarning,
                Product = products,
                ApplicationUser = users,
                OrderHeader = orderHeaders,
                Category = categories,
            };
            return View(dashboardVm);
        }
        private double GetTotalAmount()
        {
            double totalAmount=0;
            IEnumerable<OrderHeader> headers = _unitOfWork.OrderHeader.GetAll().ToList();
            foreach(var amount in headers)
            {
                totalAmount += amount.OrderTotal;
            }
            return totalAmount;
        }
        private int GetOrderTotalCount()
        {
           int totalCount = 0;
            IEnumerable<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll().ToList();
            totalCount = orderHeaders.Count();
            return totalCount;
        }
        private int GetProductCount()
        {
            int totalCount = 0;
            IEnumerable<Product> products  = _unitOfWork.Product.GetAll().ToList();
            totalCount= products.Count();
            return totalCount;
        }
        private double GetMonthlyEarning()
        {
            double monthlyEarning = 0;
            DateTime today = DateTime.Today;

            // Subtract 7 days from today
            DateTime sevenDaysAgo = today.AddDays(-7);
            IEnumerable<OrderHeader> ordersLast7Days = _unitOfWork.OrderHeader
            .GetAll(u => u.OrderDate >= sevenDaysAgo && u.OrderDate <= today)
            .ToList();
            foreach(var orderHeader in ordersLast7Days)
            {
                monthlyEarning += orderHeader.OrderTotal;
            }
            return monthlyEarning;
        }
    }
}
