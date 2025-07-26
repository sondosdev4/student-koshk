using StudentSuplier.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CartItem
{
    [Key]
    public int CartItemId { get; set; }

    [Required]
    public int ProductId { get; set; }

   
    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public decimal Price { get; set; }  // السعر عند الإضافة للسلة

    [Required(ErrorMessage = "الكمية مطلوبة")]
    [Range(1, 100, ErrorMessage = "الكمية يجب أن تكون بين 1 و 100")]
    public int Quantity { get; set; } = 1;

    [NotMapped]  // هذه الخاصية غير مخزنة في قاعدة البيانات
    public decimal TotalPrice
    {
        get
        {
            return Price * Quantity;
        }
    }



}
