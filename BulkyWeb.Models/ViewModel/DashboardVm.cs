using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models.ViewModel
{
    public class DashboardVm
    {
        public List<ApplicationUser> ApplicationUser { get; set; }
        public List<OrderHeader> OrderHeader { get; set; }
        public List<Product> Product { get; set; }

        public List<Category> Category { get; set; }

        public int OrdersCount { get; set; }

        public double OrderTotalAmount { get; set; }
        public double MonthlyRevenue { get; set; }
        
        public int UserCount { get; set; }
        public int ProductCount { get; set; }
       public List<SalesReport> SalesReport { get; set; }
    }
}
