using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSuplier.Models
{
    public class Library
    {
        [Key]
        public int LibraryId { get; set; }

        [Required, StringLength(150)]
        public string LibraryName { get; set; }

        [Required,StringLength(300)]
        public string Location { get; set; }

        [Required,StringLength(300)]
        public string Phone { get; set; }
       
        [Required]
        public string WorkingHour { get; set; }        

        [StringLength(500)]
        public string ImageUrl { get; set; }

        public bool? isActive { get; set; }

        public List<Product> Products { get; set; }

        public int UserId { get; set; }
    }
}