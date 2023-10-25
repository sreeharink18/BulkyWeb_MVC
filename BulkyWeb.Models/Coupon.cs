using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [Required]
        public int MinAmout { get; set; }
        public int DiscountAmout { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(6)]
        public string CouponCode { get; set; }

        public string? IsValid { get; set; }

    }
}
