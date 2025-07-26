using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSuplier.Data;
using StudentSuplier.Models;

namespace StudentSuplier.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Dashboard(int libraryId)
        {
            var products = await _context.Products
                            .Where(p => p.LibraryId == libraryId)
                            .ToListAsync();

            var orders = await _context.Orders
                            .Include(o => o.User)
                            .Include(o => o.Cart)
                            .Where(o => o.LibraryID == libraryId)
                            .ToListAsync();

            var model = new DashboardViewModel
            {
                Products = products,
                Orders = orders
            };

            return View(model);
        }
    }
}
