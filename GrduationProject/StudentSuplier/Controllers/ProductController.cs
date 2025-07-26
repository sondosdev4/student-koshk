using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSuplier.Data;
using StudentSuplier.Models;
using System;
using System.Data;
using System.Data.SqlClient;    
using System.Linq;

namespace StudentSuplier.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<LibraryController> _logger;

        public ProductController(IConfiguration configuration, ApplicationDbContext context, IWebHostEnvironment environment, ILogger<LibraryController> logger)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _environment = environment;
            _logger = logger;
        }


        [HttpGet]
        public IActionResult AddProducts()
        {
            Library library = GetLibraryByUser();
            var model = new AddProductsViewModel();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = "SELECT DISTINCT Category FROM Items WHERE LibraryID = @LibraryID AND Category IS NOT NULL AND Category <> ''";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@LibraryID", library.LibraryId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                var categories = new List<string>();
                while (reader.Read())
                {
                    categories.Add(reader["Category"].ToString());
                }
                con.Close();

                model.ExistingCategories = categories;
            }
            ViewBag.username = HttpContext.Session.GetString("userName");
            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> AddProducts(ProductViewModel model)
        {

            Library data = GetLibraryByUser();

            string category = model.Category;
            // جلب القيمة من الحقل النصي إذا اختار المستخدم "تصنيف جديد"
            var newCategory = HttpContext.Request.Form["NewCategory"].ToString();

            if (category == "__new__" && !string.IsNullOrWhiteSpace(newCategory))
            {
                category = newCategory.Trim();
            }


            // حفظ الصورة
            string imageUrl = null;
            var file = HttpContext.Request.Form.Files;
            if (file.Count > 0)
            {
                var fileName = Path.GetFileName(file[0].FileName);
                var filePath = Path.Combine("wwwroot/uploads/products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file[0].CopyToAsync(stream);
                }
                imageUrl = "/uploads/products/" + fileName;
            }

            string sql = "";


            sql = @"INSERT INTO Items(Name, description, price, imgurl, category, isActive, LibraryID)
            VALUES (@Name, @Description, @Price, @ImageUrl, @Category, 1, @LibraryID)";


            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Name", model.ProductName);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@Price", model.Price);
                cmd.Parameters.AddWithValue("@ImageUrl", imageUrl ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Category", category);
                cmd.Parameters.AddWithValue("@LibraryID", data.LibraryId);

                con.Open();
                await cmd.ExecuteNonQueryAsync();
                con.Close();
            }

            TempData["SuccessMessage"] = "تم إضافة المنتج إلى المكتبة";
            return RedirectToAction("AddProducts", new { libraryId = data.LibraryId });
        }




        private List<string> LoadCategories(int libraryId)
        {
            var categories = new List<string>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT DISTINCT Category FROM Items WHERE LibraryId = @LibraryId";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@LibraryId", libraryId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var cat = reader["Category"]?.ToString();
                    if (!string.IsNullOrEmpty(cat))
                        categories.Add(cat);
                }
                con.Close();
            }

            return categories;
        }


        [HttpGet]
        public IActionResult AddCategory(int libraryId)
        {
            var model = new AddProductsViewModel
            {
                Categories = LoadCategories(libraryId),
                ExistingCategories = LoadCategories(libraryId) // <-- أضف هذا لو كنت تستخدمه في الـ View
            };

            ViewBag.username = HttpContext.Session.GetString("userName"); // اسم المستخدم المسجل
            return View(model);
        }



        [HttpPost]
        public IActionResult AddCategory(int libraryId, ProductViewModel model)
        {
            // إذا كان المستخدم كتب تصنيف جديد
            if (!string.IsNullOrEmpty(model.Category))
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string sql = @"INSERT INTO Items (Name, Description, Price, ImgUrl, Category, IsActive, LibraryID)
                           VALUES (N'منتج تجريبي', N'وصف', 0, NULL, @Category, 0, @LibraryId)";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@Category", model.Category);
                    cmd.Parameters.AddWithValue("@LibraryId", libraryId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                TempData["SuccessMessage"] = "تمت إضافة التصنيف الجديد بنجاح.";
            }

            // إعادة تعبئة التصنيفات وعرض الصفحة مجددًا
            model.ExistingCategories = LoadCategories(libraryId);
            ViewBag.LibraryId = libraryId;
            return View(model);
        }





        [HttpGet]
        [HttpGet]
        public IActionResult MyProducts(int? libraryId, string? category)
        {
            if (HttpContext.Session.GetString("userId") != null)
            {
                CartController obj = new CartController(_configuration);
                ViewBag.cart = obj.getUserCart(HttpContext.Session.GetString("userId") + "");
            }

            var data1 = GetLibraryByUser();
            if (data1 == null)
                return RedirectToAction("SetupLibrary", "Library");

            SqlConnection connection = new SqlConnection(_connectionString);

            string sql = "SELECT item_Id, Name, description, price, imgurl, category, isactive, LibraryID FROM Items WHERE LibraryID = " + data1.LibraryId;
            if (!string.IsNullOrEmpty(category))
                sql += " AND Category = N'" + category + "'";

            SqlCommand cmd1 = new SqlCommand(sql, connection);
            connection.Open();
            SqlDataReader reader = cmd1.ExecuteReader();

            List<Product> s = new List<Product>();

            while (reader.Read())
            {
                s.Add(new Product
                {
                    LibraryId = data1.LibraryId,
                    ProductId = Convert.ToInt32(reader["item_Id"]),
                    ProductName = reader["Name"].ToString(),
                    Description = reader["description"].ToString(),
                    Price = Convert.ToDecimal(reader["price"]),
                    ImageUrl = reader["imgurl"].ToString(),
                    Category = reader["category"].ToString(),
                    isactive = Convert.ToBoolean(reader["isactive"]) ? "" : "ملغي"
                });
            }

            connection.Close();

            // إرسال التصنيفات الحالية إلى الـ View
            var categories = s.Select(p => p.Category).Distinct().ToList();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;

            ViewBag.username = HttpContext.Session.GetString("userName");
            return View(s);
        }



        [HttpPost]
        public IActionResult MyProducts(string? category)
        {
            if (HttpContext.Session.GetString("userId") != null)
            {
                CartController obj = new CartController(_configuration);
                ViewBag.cart = obj.getUserCart(HttpContext.Session.GetString("userId") + "");
            }

            var data1 = GetLibraryByUser();


            SqlConnection connection = new SqlConnection(_connectionString);

            string sql = "SELECT item_Id, Name, description, price, imgurl, category, LibraryID  FROM   Items  WHERE  (LibraryID = " + data1.LibraryId + ")";
            if (category != null) sql = sql + "and Category =N'" + category + "'";
            SqlCommand cmd1 = new SqlCommand(sql, connection);

            SqlDataReader reader = cmd1.ExecuteReader();


            List<Product> s = new List<Product>();

            while (reader.Read())

            {

                s.Add(new Product

                {

                    LibraryId = data1.LibraryId,
                    ProductId = Convert.ToInt32(reader["item_Id"].ToString()),
                    ProductName = reader["Name"].ToString(),

                    Description = reader["description"].ToString(),
                    Price = Convert.ToDecimal(reader["price"].ToString()),


                    ImageUrl = reader["imgurl"].ToString(),
                    Category = reader["category"].ToString(),

                });

            }

            ViewBag.username = HttpContext.Session.GetString("userName"); // اسم المستخدم المسجل
            return View(s);
        }




        private Library GetLibraryByUser()
        {
            Library data = null;

            SqlConnection connection = new SqlConnection(_connectionString);
            string sql = "SELECT * FROM Libraries WHERE user_id=" + HttpContext.Session.GetString("userId") + "";
            SqlCommand cmd = new SqlCommand(sql, connection);
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




        private Product GetProductByLibraryID( int productId)
        {
            Product data = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Items WHERE item_Id = " + productId + "";
                SqlCommand cmd = new SqlCommand(sql, connection);
               

                connection.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    data = new Product
                    {
                        ProductId = Convert.ToInt32(rdr["item_Id"]),
                        ProductName = rdr["Name"].ToString(),
                        Description = rdr["description"].ToString(),
                        Category = rdr["category"].ToString(),
                        Price = Convert.ToDecimal(rdr["price"]),
                        ImageUrl = rdr["imgurl"].ToString(),
                        IsActive = Convert.ToBoolean (rdr["IsActive"].ToString())
                    };
                }
            }

            return data;
        }



        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            Product data = GetProductByLibraryID(id);
            AddProductsViewModel model = new AddProductsViewModel();
            model.ProductId = data.ProductId;
            model.ProductName = data.ProductName;
            model.Description = data.Description;
            model.Category = data.Category ;
            model.Price = data.Price;
            model.ImageUrl = data.ImageUrl ;
            model.IsActive = data.IsActive;

            string sql = "SELECT DISTINCT Category FROM Items WHERE LibraryID = @LibraryID AND Category IS NOT NULL AND Category <> ''";
            List<string> categories = new List<string>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@LibraryID", GetLibraryByUser().LibraryId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    categories.Add(reader["Category"].ToString());
                }
                con.Close();
            }

            ViewBag.Categories = categories;


            ViewBag.username = HttpContext.Session.GetString("userName"); // اسم المستخدم المسجل
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(AddProductsViewModel model , string chk1 , int id)
        {
            Product data = GetProductByLibraryID(model.ProductId );
            // حفظ الصورة
            string imageUrl = null;
            var file = HttpContext.Request.Form.Files;
            if (file.Count > 0)
            {
                var fileName = Path.GetFileName(file[0].FileName);
                var filePath = Path.Combine("wwwroot/uploads/products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file[0].CopyToAsync(stream);
                }
                imageUrl = "/uploads/Products/" + fileName;
            }

            string sql = "";

            string active = chk1 == "active" ? "1" : "0";

            if (imageUrl !=null)

            sql = "update  Items set Name=N'" + model.ProductName + "' , description = N'" + model.Description + "', price ="+ model.Price  + ", category = N'" + model.Category + "' , imgurl = N'" + imageUrl + "', isActive=" + active + "  WHERE item_Id =" + model.ProductId;

            else
                sql = "update  Items set Name=N'" + model.ProductName + "' , description = N'" + model.Description + "', price =" + model.Price + ", category = N'" + model.Category + "' , isActive=" + active + "  WHERE item_Id =" + model.ProductId;
            using (SqlConnection con = new SqlConnection(_connectionString))

                {

                    string sqlQuery = sql;

                    SqlCommand cmd = new SqlCommand(sqlQuery, con);



                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            return RedirectToAction("MyProducts", new { libraryId = data.LibraryId});
        }

    }
}

