﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }

        public int? MultipleAddressId { get; set; }
        [ForeignKey("MultipleAddressId")]
        [ValidateNever]
        public MultipleAddress? MultipleAddress { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public  DateTime OrderDate { get; set; }
        public  DateTime ShippingDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public double OrderTotal { get; set; }

        public string PaymentMethod { get; set; }

        public string? CouponCode { get; set; }  
        public int? CouponId { get; set; }

        public  string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        public DateTime PaymentDate { get; set; }
        public DateTime PaymentDueDate { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }


        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
       
        public string PostalCode { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
