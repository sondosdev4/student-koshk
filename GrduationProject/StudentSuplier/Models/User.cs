using System.ComponentModel.DataAnnotations;

namespace StudentSuplier.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string usertype { get; set; }  // "Student", "LibraryOwner"

       
    }
}
