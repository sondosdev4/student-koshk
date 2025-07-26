using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using StudentSuplier.Models;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.EntityFrameworkCore;
using StudentSuplier.Data;


namespace StudentSuplier.Controllers
{
    public class LibraryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<LibraryController> _logger;

        public LibraryController(IConfiguration configuration, ApplicationDbContext context, IWebHostEnvironment environment,ILogger<LibraryController> logger)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _environment = environment;
            _logger = logger;
        }


        public IActionResult Details(int id, string search)
        {
            if (HttpContext.Session.GetString("userId") != null)
            {
                CartController obj = new CartController(_configuration);
                ViewBag.cart = obj.getUserCart(HttpContext.Session.GetString("userId") + "");
                ViewBag.userName = HttpContext.Session.GetString("userName");
            }

            ViewBag.Search = search; // عرض الكلمة في حقل البحث لاحقًا

            LibraryDetailViewModel data = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // جلب بيانات المكتبة
                string sql = "SELECT * FROM Libraries WHERE LibraryId = @id";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            data = new LibraryDetailViewModel
                            {
                                LibraryId = Convert.ToInt32(rdr["LibraryId"]),
                                LibraryName = rdr["LibraryName"].ToString(),
                                Location = rdr["Location"].ToString(),
                                Phone = rdr["Phone"].ToString(),
                                WorkingHour = rdr["WorkingHours"].ToString(),
                                ImageUrl = rdr["imgurl"].ToString()
                            };
                        }
                    }
                }

                // جلب المنتجات الخاصة بالمكتبة مع دعم البحث
                List<Product> s = new List<Product>();
                sql = "SELECT item_Id, Name, description, price, imgurl, category, LibraryID FROM Items " +
                      "WHERE isactive = 1 AND LibraryID = @libId";

                if (!string.IsNullOrWhiteSpace(search))
                {
                    sql += " AND Name LIKE N'%' + @search + '%'";
                }

                using (SqlCommand cmd1 = new SqlCommand(sql, connection))
                {
                    cmd1.Parameters.AddWithValue("@libId", id);
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        cmd1.Parameters.AddWithValue("@search", search);
                    }

                    using (SqlDataReader reader = cmd1.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            s.Add(new Product
                            {
                                LibraryId = id,
                                ProductId = Convert.ToInt32(reader["item_Id"]),
                                ProductName = reader["Name"].ToString(),
                                Description = reader["description"].ToString(),
                                Price = Convert.ToDecimal(reader["price"]),
                                ImageUrl = reader["imgurl"].ToString(),
                                Category = reader["category"].ToString(),
                            });
                        }
                    }
                }

                data.Products = s;
            }

            return View(data);
        }




        private Library GetLibraryByUser()
        {
            Library data = null;

            SqlConnection connection = new SqlConnection(_connectionString);
            string sql = "SELECT * FROM Libraries WHERE user_id=" + HttpContext.Session.GetString("userId") + "";
            SqlCommand cmd = new SqlCommand(sql, connection);
            if (connection.State ==  ConnectionState.Closed)
                connection.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                data = new Library
                {
                    LibraryId = Convert.ToInt32(rdr["LibraryId"]),

                    LibraryName = rdr["LibraryName"].ToString(),
                    WorkingHour = rdr["WorkingHours"].ToString(),
                    Location = rdr["Location"].ToString(),
                    Phone = rdr["Phone"].ToString(),
                    ImageUrl = rdr["imgurl"].ToString(),

                };
            }

            return data;

        }



        [HttpGet]
        public IActionResult SetupLibrary()
        {
            LibraryViewModel model = new LibraryViewModel();
            Library data = GetLibraryByUser ();
           
            if (data == null)
            {
              model.Library_id = 0;
                model.LibraryName = "";
                model.Location = "";
                model.Phone = "";
                model.ImagePath = "";
                
                model.WorkingHours = "";
                
                ViewBag.username = HttpContext.Session.GetString("userName"); // اسم المستخدم المسجل
                
               return View(model);
            }



          //  LibraryViewModel model = new LibraryViewModel();
            model.LibraryName = data.LibraryName;
            model.Location = data.Location; 
            model.Phone = data.Phone;
            model.ImagePath = data.ImageUrl;
            model.Library_id = data.LibraryId;
            model.WorkingHours = data.WorkingHour;
            ViewBag.username = HttpContext.Session.GetString("userName"); // اسم المستخدم المسجل
            
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetupLibrary(LibraryViewModel model)
        {
            try
            {
                // تسجيل بيانات النموذج للتحقق
                _logger.LogInformation($"Model received: {model.LibraryName}, {model.Location}, {model.Phone}");

                string imagePath = null;
                if (model.ImageUrl != null && model.ImageUrl.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(model.ImageUrl.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ImageUrl", "نوع الملف غير مسموح به");
                        return View(model);
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "libraries");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageUrl.CopyToAsync(fileStream);
                    }

                    imagePath = $"/uploads/libraries/{uniqueFileName}";
                    _logger.LogInformation($"Image saved at: {imagePath}");
                }

                string sql = "";
                Library existingLibrary = GetLibraryByUser();
                int userId = int.Parse(HttpContext.Session.GetString("userId"));

                if (existingLibrary == null)
                {
                    sql = $@"
                INSERT INTO Libraries 
                (LibraryName, Location, Phone, imgurl, WorkingHours, IsActive, User_Id) 
                VALUES 
                (N'{model.LibraryName}', N'{model.Location}', N'{model.Phone}', 
                N'{imagePath}', N'{model.WorkingHours}', 1, {userId})";
                }
                else
                {
                    sql = "UPDATE Libraries SET " +
                          "LibraryName = N'" + model.LibraryName + "', " +
                          "Location = N'" + model.Location + "', " +
                          "Phone = N'" + model.Phone + "', " +
                          (imagePath != null ? "imgurl = N'" + imagePath + "', " : "") +
                          "WorkingHours = N'" + model.WorkingHours + "' " +
                          "WHERE user_id = " + userId;
                }
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }


                Library library = GetLibraryByUser();
                

                return RedirectToAction("MyProducts", "Product", new { libraryId = library.LibraryId });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while saving library");
                ModelState.AddModelError("", "خطأ في قاعدة البيانات. قد تكون بعض البيانات مكررة.");
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "IO error while saving image");
                ModelState.AddModelError("ImageUrl", "خطأ في حفظ الصورة على السيرفر");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                ModelState.AddModelError("", "حدث خطأ غير متوقع: " + ex.Message);
           }

            return View(model);
        }
 
    }
}


