using Microsoft.AspNetCore.Identity;
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
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string StreetAddress { get; set; }
        public string State { get; set; }
        public int? CompanyId { get; set; }
        [ValidateNever]
        [ForeignKey("CompanyId")]
        public Company? company { get; set; }

        public int? Wallet { get; set; }
    }
}
