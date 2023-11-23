using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Product Name")]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [DisplayName("Display Order")]
        [Range(0, 100, ErrorMessage = " Display Order must between 1-100")]
        public int DisplayOrder { get; set; }
        public int CountCategory { get; set; }
        public bool List { get; set; }
        public string IsDiscount { get; set; }
        public int DiscountAmount { get; set; }
    } 
}
