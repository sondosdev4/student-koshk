using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace StudentSuplier.Models
{
    public class LibraryViewModel
    {
        public int Library_id { get; set; }

        [Required(ErrorMessage = "اسم المكتبة مطلوب")]
        public string LibraryName { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "الموقع مطلوب")]
        public string Location { get; set; }

        [Required(ErrorMessage = "صورة المكتبة مطلوبة")]
        public IFormFile ImageUrl { get; set; }

        [Required(ErrorMessage = "ساعات العمل مطلوبة")]
        public string WorkingHours { get; set; }

        public string ImagePath { get; set; }     


    }
}
