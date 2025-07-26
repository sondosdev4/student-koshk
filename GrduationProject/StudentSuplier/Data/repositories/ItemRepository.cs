using StudentSuplier.Data.Repositories;
using System.Data;
using System.Data.SqlClient;
using StudentSuplier.Models;

namespace StudentSuplier.Data.repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly IDataAccess _dataAccess;

        public ItemRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            string query = "SELECT * FROM Items WHERE item_Id = @Id";
            var parameters = new SqlParameter[] { new("@Id", id) };

            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return MapDataRowToItem(dt.Rows[0]);
            }
            return null;
        }

        private Product MapDataRowToItem(DataRow row)
        {
            return new Product
            {
                ProductId = Convert.ToInt32(row["item_Id"]),
                ProductName = row["Name"].ToString(),
                Description = row["description"].ToString(),
                Price = Convert.ToDecimal(row["price"]),
                ImageUrl = row["imgurl"].ToString(),
                Category = row["category"].ToString()
            };
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            string query = "SELECT * FROM Items";
            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, null);

            List<Product> items = new List<Product>();

            foreach (DataRow row in dt.Rows)
            {
                items.Add(new Product
                {
                    ProductId = Convert.ToInt32(row["item_Id"]),
                    ProductName = row["Name"].ToString(),
                    Description = row["description"].ToString(),
                    Price = Convert.ToDecimal(row["price"]),
                    ImageUrl = row["imgurl"].ToString(),
                    Category = row["category"].ToString()
                });
            }

            return items;
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            string query = "SELECT * FROM Items WHERE category = @Category";
            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Category", category)
            };

            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, parameters);
            List<Product> items = new List<Product>();

            foreach (DataRow row in dt.Rows)
            {
                items.Add(new Product
                {
                    ProductId = Convert.ToInt32(row["item_Id"]),
                    ProductName = row["Name"].ToString(),
                    Description = row["description"].ToString(),
                    Price = Convert.ToDecimal(row["price"]),
                    ImageUrl = row["imgurl"].ToString(),
                    Category = row["category"].ToString()
                });
            }

            return items;
        }


        public async Task<IEnumerable<Product>> SearchAsync(string keyword)
        {
            string query = @"
        SELECT * FROM Items
        WHERE Name LIKE @Keyword OR Description LIKE @Keyword OR Category LIKE @Keyword";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Keyword", $"%{keyword}%")
            };

            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, parameters);
            List<Product> products = new List<Product>();

            foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    ProductId = Convert.ToInt32(row["item_Id"]),
                    ProductName = row["Name"].ToString(),
                    Description = row["description"].ToString(),
                    Price = Convert.ToDecimal(row["price"]),
                    ImageUrl = row["imgurl"].ToString(),
                    Category = row["category"].ToString()
                });
            }

            return products;
        }

        public async Task<int> AddAsync(Product item)
        {
            string query = @"
        INSERT INTO Items (Name, description, price, imgurl, category)
        VALUES (@Name, @Description, @Price, @ImageUrl, @Category);
        SELECT SCOPE_IDENTITY();"; // لإرجاع الـ ID الجديد

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Name", item.ProductName),
        new SqlParameter("@Description", item.Description),
        new SqlParameter("@Price", item.Price),
        new SqlParameter("@ImageUrl", item.ImageUrl),
        new SqlParameter("@Category", item.Category)
            };

            object result = await _dataAccess.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result);
        }


        public async Task<bool> UpdateAsync(Product item)
        {
            string query = @"
        UPDATE Items
        SET Name = @Name,
            description = @Description,
            price = @Price,
            imgurl = @ImageUrl,
            category = @Category
        WHERE item_Id = @ItemId";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Name", item.ProductName),
        new SqlParameter("@Description", item.Description),
        new SqlParameter("@Price", item.Price),
        new SqlParameter("@ImageUrl", item.ImageUrl),
        new SqlParameter("@Category", item.Category),
        new SqlParameter("@ItemId", item.ProductId)
            };

            int rowsAffected = await _dataAccess.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }


        public async Task<bool> ExistsInLibraryAsync(int itemId, int libraryId)
        {
            string query = @"
        SELECT COUNT(*) FROM Library_Items 
        WHERE item_Id = @ItemId AND LibraryID = @LibraryId";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@ItemId", itemId),
        new SqlParameter("@LibraryId", libraryId)
            };

            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count > 0)
            {
                int count = Convert.ToInt32(dt.Rows[0][0]);
                return count > 0;
            }

            return false;
        }

        public async Task<decimal> GetAveragePriceByCategoryAsync(string category)
        {
            string query = @"
        SELECT AVG(price) 
        FROM Items 
        WHERE category = @Category";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Category", category)
            };

            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, parameters);

            if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
            {
                return Convert.ToDecimal(dt.Rows[0][0]);
            }

            return 0;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            string query = "DELETE FROM Items WHERE item_Id = @ItemId";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@ItemId", id)
            };

            int rowsAffected = await _dataAccess.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }


    }
}
