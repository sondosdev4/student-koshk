using System.ComponentModel.DataAnnotations;

namespace StudentSuplier.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "أدخل بريد إلكتروني صحيح")]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "يرجى تحديد نوع المستخدم")]
        public string UserType { get; set; }
    }
}
