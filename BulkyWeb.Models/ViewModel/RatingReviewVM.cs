using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models.ViewModel
{
    public class RatingReviewVM
    {
        public List<RatingReview> RatingReviewAll { get; set; }
        public int ProductId { get; set; }
        public RatingReview RatingReview { get; set; }
        public int RatingValue { get; set; }
        public string Review { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
