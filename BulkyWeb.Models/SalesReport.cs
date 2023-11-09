using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models
{
    public class SalesReport
    {
        [Key]
        public int Id { get; set; } 
        public DateTime? Month { get; set; }
        public DateTime? Day { get; set; }
        public DateTime? Week { get; set; }
        public int TotalAmount { get; set; }
    }
}
