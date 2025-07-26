using StudentSuplier.Models;
using System.Data;
using System.Data.SqlClient;

namespace StudentSuplier.Data.repositories
{
    // Example repository interface
    public interface ILibraryRepository
    {
        Library GetLibraryById(int id);
        IEnumerable<Library> GetActiveLibraries();
        int AddLibrary(Library library);
        bool UpdateLibrary(Library library);
        
        Task<bool> DeleteAsync(int id);
        Task<bool> ToggleStatusAsync(int libraryId);
        Task<IEnumerable<Library>> SearchLibrariesAsync(string searchTerm, bool includeInactive, int page, int pageSize);
        // Domain-specific methods like:
        IEnumerable<Product> GetLibraryProducts(int libraryId);
        decimal CalculateLibraryRevenue(int libraryId, DateTime startDate, DateTime endDate);
    }

    
}
