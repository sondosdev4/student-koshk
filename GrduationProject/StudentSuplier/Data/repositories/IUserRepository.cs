using StudentSuplier.Models;
using System.Data;
using System.Data.SqlClient;
namespace StudentSuplier.Data.repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<int> AddAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int userId);
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
        Task<User> AuthenticateAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string newPassword);
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, int page, int pageSize);
    }

    

}
