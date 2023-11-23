using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models.ViewModel
{
	public class OrderVM
	{
		public OrderHeader OrderHeader { get; set; }
		public Coupon Coupon { get; set; }
		public IEnumerable<OrderDetail> OrderDetails { get; set; }
		public ApplicationUser User { get; set; }

		public double OrginalTotalAmount { get; set; }
	}
}
