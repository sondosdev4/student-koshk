using Microsoft.AspNetCore.Identity;
using StudentSuplier.Models;
using System.Data;
using System.Data.SqlClient;
using StudentSuplier.Data.Repositories;

namespace StudentSuplier.Data.repositories
{
    public class ShoppingCartRepository :IShoppingCartRepository
    {
        private readonly IDataAccess _dataAccess;

        public ShoppingCartRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        // -- دوال IItemRepository موجودة هنا --

        // دوال IShoppingCartRepository:

        public async Task<IShoppingCartRepository> GetByUserIdAsync(int userId)
        {
            // على الأغلب تقصد ترجع سلة المستخدم، فالأفضل يرجع نوع ShoppingCart وليس IRepository
            // لكن حسب تعريفك:
            // من غير تنفيذ كامل، أرجع null مؤقتاً
            return null;
        }

        public async Task<CartItem> GetCartItemAsync(int cartId, int itemId)
        {
            string query = @"SELECT * FROM Shopping_Cart WHERE CartId = @CartId AND ItemId = @ItemId";
            var parameters = new SqlParameter[]
            {
            new SqlParameter("@CartId", cartId),
            new SqlParameter("@ItemId", itemId)
            };

            var dt = await _dataAccess.ExecuteQueryAsync(query, parameters);

            if (dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];
            return new CartItem
            {
                CartItemId = Convert.ToInt32(row["CartId"]),
                ProductId = Convert.ToInt32(row["ItemId"]),
                Quantity = Convert.ToInt32(row["Quantity"])
            };
        }

        public async Task<int> AddItemToCartAsync(int userId, int itemId, int quantity)
        {
            string query = @"INSERT INTO Shopping_Cart (UserId, ItemId, Quantity) VALUES (@UserId, @ItemId, @Quantity)";
            var parameters = new SqlParameter[]
            {
            new SqlParameter("@UserId", userId),
            new SqlParameter("@ItemId", itemId),
            new SqlParameter("@Quantity", quantity)
            };

            return await _dataAccess.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task<bool> UpdateCartItemQuantityAsync(int cartId, int itemId, int newQuantity)
        {
            string query = @"UPDATE Shopping_Cart SET Quantity = @Quantity WHERE CartId = @CartId AND ItemId = @ItemId";
            var parameters = new SqlParameter[]
            {
            new SqlParameter("@Quantity", newQuantity),
            new SqlParameter("@CartId", cartId),
            new SqlParameter("@ItemId", itemId)
            };

            int rowsAffected = await _dataAccess.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> RemoveItemFromCartAsync(int cartId, int itemId)
        {
            string query = @"DELETE FROM Shopping_Cart WHERE CartId = @CartId AND ItemId = @ItemId";
            var parameters = new SqlParameter[]
            {
            new SqlParameter("@CartId", cartId),
            new SqlParameter("@ItemId", itemId)
            };

            int rowsAffected = await _dataAccess.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            string query = @"DELETE FROM Shopping_Cart WHERE UserId = @UserId";
            var parameters = new SqlParameter[]
            {
            new SqlParameter("@UserId", userId)
            };

            int rowsAffected = await _dataAccess.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<decimal> CalculateCartTotalAsync(int userId)
        {
            string query = @"
            SELECT SUM(i.Price * sc.Quantity) AS Total
            FROM Shopping_Cart sc
            INNER JOIN Items i ON sc.ItemId = i.item_Id
            WHERE sc.UserId = @UserId";

            var parameters = new SqlParameter[]
            {
            new SqlParameter("@UserId", userId)
            };

            var dt = await _dataAccess.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count == 0 || dt.Rows[0]["Total"] == DBNull.Value)
                return 0;

            return Convert.ToDecimal(dt.Rows[0]["Total"]);
        }

        public async Task<bool> CheckoutAsync(int userId, int libraryId)
        {
            // مثال مبسط جداً لعملية الدفع:
            // 1. حساب المجموع
            decimal total = await CalculateCartTotalAsync(userId);

            // 2. إضافة طلب في جدول Orders (مفترض أنك تملك جدول Orders)
            string insertOrderQuery = @"
            INSERT INTO Orders (UserId, LibraryId, TotalAmount, OrderDate)
            VALUES (@UserId, @LibraryId, @TotalAmount, @OrderDate);
            SELECT SCOPE_IDENTITY();";

            var orderParams = new SqlParameter[]
            {
            new SqlParameter("@UserId", userId),
            new SqlParameter("@LibraryId", libraryId),
            new SqlParameter("@TotalAmount", total),
            new SqlParameter("@OrderDate", DateTime.Now)
            };

            var dt = await _dataAccess.ExecuteQueryAsync(insertOrderQuery, orderParams);
            if (dt.Rows.Count == 0)
                return false;

            int orderId = Convert.ToInt32(dt.Rows[0][0]);

            // 3. نسخ عناصر السلة إلى Order_Items مثلاً
            string insertOrderItemsQuery = @"
            INSERT INTO Order_Items (OrderId, ItemId, Quantity)
            SELECT @OrderId, ItemId, Quantity FROM Shopping_Cart WHERE UserId = @UserId";

            var orderItemsParams = new SqlParameter[]
            {
            new SqlParameter("@OrderId", orderId),
            new SqlParameter("@UserId", userId)
            };

            await _dataAccess.ExecuteNonQueryAsync(insertOrderItemsQuery, orderItemsParams);

            // 4. حذف السلة بعد إتمام الطلب
            await ClearCartAsync(userId);

            return true;
        }
    }

}
