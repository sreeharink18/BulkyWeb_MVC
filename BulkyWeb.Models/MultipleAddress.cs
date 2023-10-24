using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models
{
	public class MultipleAddress
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[DisplayName("Phone Number")]
		[StringLength(10, MinimumLength = 10, ErrorMessage = "The phone number must be 10 digits.")]
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
        public string? ApplicationUserId { get; set; }	
        public int? Options { get; set; }
    }
}
