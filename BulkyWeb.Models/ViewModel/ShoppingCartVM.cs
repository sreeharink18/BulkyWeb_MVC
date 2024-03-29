﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models.ViewModel
{
    public class ShoppingCartVM
    {
       
        public IEnumerable<ShoppingCart> shoppingCartsList { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public MultipleAddress MultipleAddress { get; set; }
        
        public ApplicationUser ApplicationUser { get; set; }
        public List<Coupon> CouponList { get; set; }

    }
}
