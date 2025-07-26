using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentSuplier.Data.repositories;

namespace StudentSuplier.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IUserRepository userRepository,
            ILibraryRepository libraryRepository,
            ILogger<AdminController> logger)
        {
            _userRepository = userRepository;
            _libraryRepository = libraryRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region User Management
        public async Task<IActionResult> Users(string searchTerm = "", int page = 1, int pageSize = 10)
        {
            try
            {
                ViewBag.SearchTerm = searchTerm;
                var users = await _userRepository.SearchUsersAsync(searchTerm, page, pageSize);
                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                TempData["ErrorMessage"] = "An error occurred while retrieving users.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var success = await _userRepository.DeleteAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "User deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found or could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with ID {id}");
                TempData["ErrorMessage"] = "An error occurred while deleting the user.";
            }

            return RedirectToAction(nameof(Users));
        }
        #endregion

        #region Library Management
        public async Task<IActionResult> Libraries(string searchTerm = "", bool includeInactive = false, int page = 1, int pageSize = 10)
        {
            try
            {
                ViewBag.SearchTerm = searchTerm;
                ViewBag.IncludeInactive = includeInactive;
                var libraries = await _libraryRepository.SearchLibrariesAsync(searchTerm, includeInactive, page, pageSize);
                return View(libraries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving libraries");
                TempData["ErrorMessage"] = "An error occurred while retrieving libraries.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLibrary(int id)
        {
            try
            {
                var success = await _libraryRepository.DeleteAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Library deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Library not found or could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting library with ID {id}");
                TempData["ErrorMessage"] = "An error occurred while deleting the library.";
            }

            return RedirectToAction(nameof(Libraries));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLibraryStatus(int id)
        {
            try
            {
                var success = await _libraryRepository.ToggleStatusAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Library status updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Library not found or status could not be updated.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling status for library with ID {id}");
                TempData["ErrorMessage"] = "An error occurred while updating library status.";
            }

            return RedirectToAction(nameof(Libraries));
        }
        #endregion
    }
}
