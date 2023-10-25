using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models.ViewModel
{
    public  class CouponVM
    {
        public  Coupon Coupon { get; set; }
        public IEnumerable< ApplicationUser> ApplicationUser { get; set; }
    }
}
