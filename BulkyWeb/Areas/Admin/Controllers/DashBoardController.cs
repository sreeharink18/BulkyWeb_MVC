using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
            MonthTotal();
            double totalAmount = GetTotalAmount();
            int OrderCount = GetOrderTotalCount();
            int ProductCount = GetProductCount();
            double MonthlyEarning = GetMonthlyEarning();
            int usercount = UserCount();
            List<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll().ToList();
            List<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll().ToList();
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            List<Product> products = _unitOfWork.Product.GetAll().ToList();
            List<SalesReport> salesReports = _unitOfWork.SalesReport.GetAll().ToList();
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
                SalesReport = salesReports,
                UserCount = usercount,
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
        public int UserCount()
        {
            int count = 0;
            List<ApplicationUser> applicationUsers = _unitOfWork.ApplicationUser.GetAll().ToList();
            count = applicationUsers.Count();
            return count;
        }
        public void MonthTotal()
        {
            IEnumerable<OrderHeader> orderHeaders = _unitOfWork.OrderHeader .GetAll().ToList();
            var monthlySales = orderHeaders
        .GroupBy(o => o.OrderDate.Month)
        .Select(group => new SalesReport
        {
            Month =  new DateTime(DateTime.Now.Year, group.Key, 1),
            TotalAmount = (int)group.Sum(o => o.OrderTotal)
        })
        .ToList();

            foreach (var sale in monthlySales)
            {
                // Check if the record already exists for the month
                var existingRecord = _unitOfWork.SalesReport.Get(sr => sr.Month == sale.Month);

                if (existingRecord != null)
                {
                    // Update existing record
                    existingRecord.TotalAmount = sale.TotalAmount;
                    _unitOfWork.SalesReport.Update(existingRecord);
                }
                else
                {
                    // Add new record if it doesn't exist
                    _unitOfWork.SalesReport.Add(sale);
                }
                _unitOfWork.Save();
            
            }

        }
    }
}
