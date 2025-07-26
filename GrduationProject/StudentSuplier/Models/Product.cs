using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSuplier.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required, StringLength(200)]
        public string ProductName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
        [Required]
        public string Category { get; set; }

        [Required]
        public decimal? Price { get; set; }

        [Required]
        public int LibraryId { get; set; }

        [ForeignKey("LibraryId")]
        public Library Library { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; } // مسار الصورة

        public string? isactive { get; set; }

        public bool IsActive { get; set; }
    }
}