using Microsoft.AspNetCore.Identity;
using StudentSuplier.Models;
using System.Data;
using System.Data.SqlClient;
using StudentSuplier.Data.Repositories;

namespace StudentSuplier.Data.repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IDataAccess _dataAccess;

    public UserRepository(IPasswordHasher<User> passwordHasher, IDataAccess dataAccess)
    {
        _passwordHasher = passwordHasher;
        _dataAccess = dataAccess;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
    {
        // تهيئة كائن مستخدم مؤقت فقط للتجزئة
        var tempUser = new User { UserId = userId };
        var hashedPassword = _passwordHasher.HashPassword(tempUser, newPassword);

        string query = @"UPDATE Users SET password = @Password WHERE user_Id = @UserId";

        var parameters = new SqlParameter[]
        {
            new SqlParameter("@Password", hashedPassword),
            new SqlParameter("@UserId", userId)
        };

        int rowsAffected = await _dataAccess.ExecuteNonQueryAsync(query, parameters);

        return rowsAffected > 0;
    }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            string query = "SELECT * FROM Users WHERE email = @Email";
            var parameters = new SqlParameter[] { new SqlParameter("@Email", email) };

            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count == 0) return null;

            var user = MapDataRowToUser(dt.Rows[0]);

            // تحقق من كلمة المرور المشفرة
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (result != PasswordVerificationResult.Success)
                return null;

            return user;
        }


        private User MapDataRowToUser(DataRow row)
        {
            return new User
            {
                UserId = Convert.ToInt32(row["user_Id"]),
                Email = row["email"].ToString(),
                Username = row["username"].ToString(),
                Password = row["password"].ToString(),
                usertype = row["usertype"].ToString()
            };
        }

        public async Task<bool> UpdateAsync(User user)
        {
            string query = @"
        UPDATE Users
        SET Username = @Username,
            Email = @Email,
            UserType = @UserType
        WHERE UserId = @UserId";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Username", user.Username),
        new SqlParameter("@Email", user.Email),
        new SqlParameter("@UserType", user.usertype),
        new SqlParameter("@UserId", user.UserId)
            };

            int rowsAffected = await _dataAccess.ExecuteNonQueryAsync(query, parameters);

            // ارجع true إذا تم تحديث سجل واحد أو أكثر، وإلا ارجع false
            return rowsAffected > 0;
        }


        public async Task<User> GetByIdAsync(int id)
        {
            string query = "SELECT * FROM Users WHERE user_Id = @UserId";
            var parameters = new SqlParameter[]
            {
        new SqlParameter("@UserId", id)
            };

            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count == 0)
                return null;

            return new User
            {
                UserId = Convert.ToInt32(dt.Rows[0]["user_Id"]),
                Email = dt.Rows[0]["email"].ToString(),
                Username = dt.Rows[0]["username"].ToString(),
                Password = dt.Rows[0]["password"].ToString(),
                usertype = dt.Rows[0]["usertype"].ToString()
            };
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            string query = "SELECT * FROM Users WHERE email = @Email";
            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Email", email)
            };

            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count == 0)
                return null;

            return new User
            {
                UserId = Convert.ToInt32(dt.Rows[0]["user_Id"]),
                Email = dt.Rows[0]["email"].ToString(),
                Username = dt.Rows[0]["username"].ToString(),
                Password = dt.Rows[0]["password"].ToString(),
                usertype = dt.Rows[0]["usertype"].ToString()
            };
        }
        public async Task<int> AddAsync(User user)
        {
            string query = @"
        INSERT INTO Users (email, username, password, usertype)
        VALUES (@Email, @Username, @Password, @Usertype);
        SELECT SCOPE_IDENTITY();";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Email", user.Email),
        new SqlParameter("@Username", user.Username),
        new SqlParameter("@Password", user.Password),
        new SqlParameter("@Usertype", user.usertype)
            };

            object result = await _dataAccess.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result);
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            string query = "DELETE FROM Users WHERE user_Id = @UserId";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@UserId", userId)
            };

            int rowsAffected = await _dataAccess.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> EmailExistsAsync(string email, int? userId = null)
        {
            string query = @"
        SELECT COUNT(*) FROM Users 
        WHERE email = @Email
        AND (@UserId IS NULL OR user_Id != @UserId)
    ";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Email", email),
        new SqlParameter("@UserId", (object)userId ?? DBNull.Value)
            };

            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, parameters);

            if (dt.Rows.Count > 0)
            {
                int count = Convert.ToInt32(dt.Rows[0][0]);
                return count > 0;
            }

            return false;
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, int page, int pageSize)
        {
            List<User> users = new List<User>();
            string query = @"
        SELECT * FROM Users
        WHERE Username LIKE @Search OR Email LIKE @Search
        ORDER BY Username
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Search", $"%{searchTerm}%"),
        new SqlParameter("@Offset", (page - 1) * pageSize),
        new SqlParameter("@PageSize", pageSize)
            };

            DataTable dt = await _dataAccess.ExecuteQueryAsync(query, parameters);

            foreach (DataRow row in dt.Rows)
            {
                users.Add(new User
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    Username = row["Username"].ToString(),
                    Email = row["Email"].ToString(),
                    Password = row["PasswordHash"].ToString(),
                });
            }

            return users;
        }


    }
}
