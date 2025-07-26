using StudentSuplier.Models;
using System.Data;
using System.Data.SqlClient;

namespace StudentSuplier.Data.repositories
{
    public interface IItemRepository
    {
        Task<Product> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> SearchAsync(string term);
        Task<int> AddAsync(Product item);
        Task<bool> UpdateAsync(Product item);
        Task<bool> DeleteAsync(int itemId);
        Task<bool> ExistsInLibraryAsync(int itemId, int libraryId);
        Task<decimal> GetAveragePriceByCategoryAsync(string category);
    }
}
