using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using StudentSuplier.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
namespace StudentSuplier.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction ("Index");
        }


        public IActionResult Index(string search)
        {
            if (HttpContext.Session.GetString("userId") != null)
            {
                CartController obj = new CartController(_configuration);
                ViewBag.cart = obj.getUserCart(HttpContext.Session.GetString("userId") + "");
                ViewBag.userName = HttpContext.Session.GetString("userName");
            }

            ViewBag.Search = search; // حتى يظهر في خانة البحث في الـ View

            List<Library> libraries = new List<Library>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Libraries WHERE IsActive = 1";

                if (!string.IsNullOrWhiteSpace(search))
                {
                    sql += " AND LibraryName LIKE N'%' + @search + '%'";
                }

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.Text;

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        cmd.Parameters.AddWithValue("@search", search);
                    }

                    if (con.State == ConnectionState.Closed)
                        con.Open();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            libraries.Add(new Library
                            {
                                LibraryId = Convert.ToInt32(rdr["LibraryId"]),
                                LibraryName = rdr["LibraryName"].ToString(),
                                Location = rdr["Location"].ToString(),
                                Phone = rdr["Phone"].ToString(),
                                ImageUrl = rdr["imgurl"].ToString(),
                                WorkingHour = rdr["WorkingHours"].ToString(),
                                isActive = Convert.ToBoolean(rdr["IsActive"].ToString())
                            });
                        }
                    }

                    con.Close();
                }
            }

            return View(libraries);
        }


    }
}
