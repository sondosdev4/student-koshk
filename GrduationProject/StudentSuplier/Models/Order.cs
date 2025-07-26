using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSuplier.Models
{
    public class Order
    {
        [Key]
        public int Order_Id { get; set; }

        [ForeignKey("User")]
        public int User_Id { get; set; }
        public User User { get; set; }

        // ربط مباشر مع الـ Cart
        [ForeignKey("Cart")]
        public int Cart_Id { get; set; }
        public CartItem Cart { get; set; }

        public decimal Total_Price { get; set; }

        [ForeignKey("Library")]
        public int LibraryID { get; set; }
        public Library Library { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "جديد";
    }
}
