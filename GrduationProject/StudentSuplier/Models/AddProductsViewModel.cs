using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSuplier.Models
{
    public class AddProductsViewModel
    {
      
        public int ProductId { get; set; }

       
        public string ProductName { get; set; }

        
        public string Description { get; set; }
      

        public string Category { get; set; } 
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> ExistingCategories { get; set; } = new List<string>();
        public decimal? Price { get; set; }


        public bool? IsActive { get; set; }

        public string? isactive { get; set; }
        public string ImageUrl { get; set; } // مسار الصورة
    }
}
