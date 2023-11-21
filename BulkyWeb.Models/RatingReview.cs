using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models
{
    public class RatingReview
    {
        [Key]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product?  Product { get; set; }

        public string? ApplicationUserId { get; set; }
       
        
        [Required]
        
        public string? Review { get; set; }
         
        public int? Rating { get; set; }

        public DateTime? TimeStamp { get; set; }    

        public ApplicationUser? User { get; set; }
        public string? Name { get; set; }

    }
}
