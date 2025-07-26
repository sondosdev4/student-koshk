using System.Data;
using StudentSuplier.Models;
using System.Data.SqlClient;
using StudentSuplier.Data.Repositories;

namespace StudentSuplier.Data.repositories
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly IDataAccess _dataAccess;

        public LibraryRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public int AddLibrary(Library library)
        {
            throw new NotImplementedException();
        }

        public decimal CalculateLibraryRevenue(int libraryId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Library> GetActiveLibraries()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Library>> SearchLibrariesAsync(string searchTerm, bool includeInactive, int page, int pageSize)
        {
            List<Library> libraries = new List<Library>();
            string query = @"
        SELECT * FROM Libraries
        WHERE (Name LIKE @Search OR Address LIKE @Search)
        AND (@IncludeInactive = 1 OR IsActive = 1)
        ORDER BY Name
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Search", $"%{searchTerm}%"),
        new SqlParameter("@IncludeInactive", includeInactive ? 1 : 0),
        new SqlParameter("@Offset", (page - 1) * pageSize),
        new SqlParameter("@PageSize", pageSize)
            };

            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, parameters);

            foreach (DataRow row in dt.Rows)
            {
                libraries.Add(new Library
                {
                    LibraryId = Convert.ToInt32(row["LibraryId"]),
                    LibraryName = row["Name"].ToString(),
                    Location = row["Address"].ToString(),
                    Phone = row["Phone"].ToString(),
                    
                });
            }

            return libraries;
        }

        public IEnumerable<Product> GetLibraryProducts(int libraryId)
        {
            string query = @"SELECT i.* FROM Items i
                     INNER JOIN Library_Items li ON i.item_Id = li.item_Id
                     WHERE li.LibraryID = @LibraryId";
            var parameters = new SqlParameter[] { new SqlParameter("@LibraryId", libraryId) };

            DataTable dt = _dataAccess.ExecuteQuery(query, parameters);
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

        public bool UpdateLibrary(Library library)
        {
            string query = @"
        UPDATE Libraries
        SET LibraryName = @LibraryName,
            Location = @Location,
            Phone = @Phone,
            imgurl = @ImgUrl,
            WorkingHours = @WorkingHours,
            IsActive = @IsActive
        WHERE LibraryId = @LibraryId";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@LibraryName", library.LibraryName),
        new SqlParameter("@Location", library.Location),
        new SqlParameter("@Phone", library.Phone),
        new SqlParameter("@ImgUrl", library.ImageUrl),
        new SqlParameter("@WorkingHours", library.WorkingHour),
        new SqlParameter("@LibraryId", library.LibraryId)
            };

            int rowsAffected = _dataAccess.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public async Task<bool> ToggleStatusAsync(int libraryId)
        {
            string query = @"
        UPDATE Libraries 
        SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END 
        WHERE LibraryId = @LibraryId";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@LibraryId", libraryId)
            };

            int rowsAffected = await _dataAccess.ExecuteNonQueryAsync(query, parameters);

            // إذا تم تحديث صف واحد على الأقل، نرجع true
            return rowsAffected > 0;
        }

        //public LibraryRepository(IDataAccess dataAccess)
        //{
        //    _dataAccess = dataAccess;
        //}

        public async Task<bool> DeleteAsync(int id)
        {
            // نفّذ حذف المكتبة
            string query = "DEL_Libraries_PRC";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@LibraryId", id)
            };

            int rowsAffected = await _dataAccess.ExecuteNonQueryAsync(query, parameters);

            return rowsAffected > 0;
        }

        public Library GetLibraryById(int id)
        {
            string query = @"SELECT * FROM Libraries WHERE LibraryId = @LibraryId";
            var parameters = new SqlParameter[] { new SqlParameter("@LibraryId", id) };

            DataTable dt = _dataAccess.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return new Library
                {
                    LibraryId = Convert.ToInt32(dt.Rows[0]["LibraryId"]),
                    LibraryName = dt.Rows[0]["LibraryName"].ToString(),
                    Location = dt.Rows[0]["Location"].ToString(),
                    Phone = dt.Rows[0]["Phone"].ToString(),
                    ImageUrl = dt.Rows[0]["imgurl"].ToString(),
                    WorkingHour = dt.Rows[0]["WorkingHours"].ToString()
                    
                };
            }
            return null;
        }

    }
}
