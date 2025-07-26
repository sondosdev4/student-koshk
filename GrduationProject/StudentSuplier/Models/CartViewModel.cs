namespace StudentSuplier.Models
{
    public class CartViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int UserId { get; set; }
        public decimal Price { get; set; }  // السعر عند الإضافة للسلة
        public int Quantity { get; set; }
        public decimal total_price { get; set; }



    }
}
