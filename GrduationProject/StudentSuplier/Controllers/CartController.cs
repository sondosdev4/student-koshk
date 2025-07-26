using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using StudentSuplier.Models;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
namespace StudentSuplier.Controllers
{
    public class CartController : Controller
    {
        private readonly IConfiguration _configuration;
        
        private readonly string _connectionString;

        public CartController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        public IActionResult Index()
        {
            CartListViewModel model = new CartListViewModel();
            if (HttpContext.Session.GetString("userId") != null)
            {
                CartController obj = new CartController(_configuration);
                ViewBag.cart = obj.getUserCart(HttpContext.Session.GetString("userId") + "");
                ViewBag.userName = HttpContext.Session.GetString("userName");
            }

            if (HttpContext.Session.GetString("userId") != null)
            {
                CartController obj = new CartController(_configuration);
                ViewBag.cart = obj.getUserCart(HttpContext.Session.GetString("userId") + "");


                SqlConnection connection = new SqlConnection(_connectionString);
                string sql = "";
                sql += " SELECT         Shopping_Cart.cart_id, Shopping_Cart.user_Id, Shopping_Cart.item_Id, Shopping_Cart.quantity, Shopping_Cart.price, Shopping_Cart.total_price, Items.Name ";
                sql += " FROM            Shopping_Cart INNER JOIN ";
                sql += " Items ON Shopping_Cart.item_Id = Items.item_Id ";
                sql += " WHERE        (Shopping_Cart.user_Id = " + HttpContext.Session.GetString("userId") + ")";

                connection.Open();
                SqlCommand cmd1 = new SqlCommand(sql, connection);

                SqlDataReader reader = cmd1.ExecuteReader();



                List<CartViewModel> s = new List<CartViewModel>();

                model.carts = s;


                while (reader.Read())

                {

                    s.Add(new CartViewModel

                    {

                        CartItemId = Convert.ToInt32(reader["cart_id"].ToString()),
                        
                        ProductId = Convert.ToInt32(reader["item_Id"].ToString()),
                        ProductName = reader["Name"].ToString(),
                        UserId = Convert.ToInt32(reader["user_Id"].ToString()),
                        total_price = Convert.ToDecimal (reader["total_price"].ToString()),
                        Price = Convert.ToDecimal(reader["price"].ToString()),


                        Quantity = Convert.ToInt32(reader["quantity"].ToString()),
                      

                    });

                }
                connection.Close();

                model.products  = GetSuggestedProduct(HttpContext.Session.GetString("userId"));

                if (model.carts.Count == 0) return RedirectToAction("Index", "Home");

                return View(model);

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

               
            
        }
        

        private List<Product> GetSuggestedProduct(string user)
        {
            string cat = getCartCategory(user);

            if (cat.Trim().Length == 0) return null;
            List<Product> s = new List<Product>();
           string  sql = "SELECT top(10) item_Id, Name, description, price, imgurl, category, LibraryID FROM Items " +
                  "WHERE isactive = 1 AND category in ("+ cat + ")";

            SqlConnection connection = new SqlConnection(_connectionString);

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            using (SqlCommand cmd1 = new SqlCommand(sql, connection))
            {
               
                

                using (SqlDataReader reader = cmd1.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        s.Add(new Product
                        {
                            LibraryId = 0,
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

            return s;
        }

        private string getCartCategory( string userid )
        {
            string sql = "SELECT DISTINCT Items.category ";
            sql += " FROM     Shopping_Cart INNER JOIN ";
            sql += " Items ON Shopping_Cart.item_Id = Items.item_Id ";
            sql += " WHERE  (Shopping_Cart.user_Id = " + userid + ")";

            string category = "";
            SqlConnection connection = new SqlConnection(_connectionString);
           
            if (connection.State ==ConnectionState.Closed )
                connection.Open();
            SqlCommand cmd1 = new SqlCommand(sql, connection);

            SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                category = category+ "N'"+ reader["category"].ToString() +"',";
            }

            if (category .Trim ().Length >0)
                category =category.Substring (0,category.Trim().Length-1);
            return category;


        }
        private int checkCart (string p , string u)
        {
            int row = 0;
            SqlConnection connection = new SqlConnection(_connectionString);
            string sql = "SELECT * FROM Shopping_Cart WHERE user_id=" + u + " and item_id =" + p + "";
            SqlCommand cmd = new SqlCommand(sql, connection);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            
            if (reader.Read())
            {
                row = Convert.ToInt32( reader["quantity"].ToString ());
            }

            return row;
        }

        

        public int getUserCart(string u)
        {
            int row = 0;
            SqlConnection connection = new SqlConnection(_connectionString);
            string sql = "SELECT * FROM Shopping_Cart WHERE user_id=" + u + "";
            SqlCommand cmd = new SqlCommand(sql, connection);
            
            if (connection.State == ConnectionState.Closed )
                     connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                row++;
            }

            return row;
        }


        [HttpPost]
        public JsonResult AddToCart(int productId, string productName, decimal price, int libraryid, int quantity = 1)
        {
            if (HttpContext.Session.GetString("userId") == null)
            {
                // إعادة JSON برسالة تطلب تسجيل الدخول
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account"), message = "يرجى تسجيل الدخول أولاً." });
            }

            string userID = HttpContext.Session.GetString("userId").ToString();

            int currentQuantity = checkCart(productId.ToString(), userID);
            int newQuantity = currentQuantity + quantity;

            string sql = "";

            if (currentQuantity == 0)
                sql = $"INSERT INTO Shopping_Cart(user_Id, item_Id, quantity, price, total_price) VALUES ({userID}, {productId}, {quantity}, {price}, {quantity * price})";
            else
                sql = $"UPDATE Shopping_Cart SET quantity = {newQuantity}, total_price = {newQuantity * price} WHERE user_id = {userID} AND item_id = {productId}";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, con);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return Json(new { success = true, message = "✅ تم إضافة المنتج إلى السلة!" });
        }





        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            if (HttpContext.Session.GetString("userId") == null)
            {
                return RedirectToAction("Login", "Account");
            }


            string sql = "delete from   Shopping_Cart where cart_id ="+ productId + "";
            using (SqlConnection con = new SqlConnection(_connectionString))

            {

                string sqlQuery = sql;

                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();

            }



            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult GetCartCount()
        {
            string userId = HttpContext.Session.GetString("userId");
            int count = 0;

            if (!string.IsNullOrEmpty(userId))
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string sql = "SELECT COUNT(*) FROM Shopping_Cart WHERE user_id = @userId";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    con.Open();
                    var result = cmd.ExecuteScalar();
                    con.Close();

                    if (result != DBNull.Value && result != null)
                    {
                        count = Convert.ToInt32(result);
                    }
                }
            }

            return Json(new { count = count });
        }






    }
}
