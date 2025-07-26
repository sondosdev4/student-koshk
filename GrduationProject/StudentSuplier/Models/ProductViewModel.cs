using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace StudentSuplier.Models
{
    public class ProductViewModel
    {
        [Required]
        public string ProductName { get; set; }

        [Required]
        public string Category { get; set; }
        public string NewCategory { get; set; }

        [Required]
        public decimal Price { get; set; }


        [Required]
        public string Description { get; set; }

        [Required]
        public IFormFile ProductImage { get; set; }

        public int LibraryId { get; set; }

        public int ProductId { get; set; }

        public bool? isActive { get; set; }

        public List<string> ExistingCategories { get; set; } = new List<string>();

    }
}
