﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyWeb.Models
{
    public class Product
    {

        [Key]
        public int Id { get; set; }
        [Required]

        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        [DisplayName("List Price ")]
        public double ListPrice { get; set; }
        [Required]
        [DisplayName("Price for 1-50")]
        [Range(1, 1000)]
        public double Price { get; set; }
        [Required]
        [DisplayName("Price for 50-100")]
        [Range(1, 1000)]
        public double Price50 { get; set; }
        [Required]
        [DisplayName("Price for 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }


        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category category { get; set; }
        public bool DisplayList { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }

        public int? TotalBooktCount { get; set; }
        public string? IsDiscountProduct {get; set; }

        [Range(1, 100, ErrorMessage = "The value must be between 1 and 100.")]
        public int? DiscountAmount { get; set; }
    }
}
