using StudentSuplier.Models;
using System.Data;
using System.Data.SqlClient;


namespace StudentSuplier.Data.repositories
{
    public interface IShoppingCartRepository
    {
        Task<IShoppingCartRepository> GetByUserIdAsync(int userId);
        Task<CartItem> GetCartItemAsync(int cartId, int itemId);
        Task<int> AddItemToCartAsync(int userId, int itemId, int quantity);
        Task<bool> UpdateCartItemQuantityAsync(int cartId, int itemId, int newQuantity);
        Task<bool> RemoveItemFromCartAsync(int cartId, int itemId);
        Task<bool> ClearCartAsync(int userId);
        Task<decimal> CalculateCartTotalAsync(int userId);
        Task<bool> CheckoutAsync(int userId, int libraryId);
    }
}
