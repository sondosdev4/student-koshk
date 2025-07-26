using Microsoft.AspNetCore.Mvc;
using StudentSuplier.Data;
using StudentSuplier.Models;
using System.Threading.Tasks;

namespace StudentSuplier.Controllers
{
    public class AccountController : Controller
    {
        private readonly EmployeeDataAccess _employeeData;
        private readonly IConfiguration _configuration;

        public AccountController(EmployeeDataAccess employeeData)
        {
            _employeeData = employeeData;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("userId") != null)
            {
                CartController obj = new CartController(_configuration);
                ViewBag.cart = obj.getUserCart(HttpContext.Session.GetString("userId") + "");

            } 
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            User user = await _employeeData.GetEmployeeById(email ,password );
            if (user == null)
            {
                ViewBag.Error = "البيانات غير صحيحة أو لم يتم التعرف على نوع المستخدم.";
                return View();
            }

            else
            {
                HttpContext.Session.SetString("userId", user.UserId + "");
                HttpContext.Session.SetString("userName", user.Username  + "");
                if (user.usertype == "الطالب")
                    return RedirectToAction("Index" , "Home");
                else
                    return RedirectToAction("MyProducts", "Product");

            }


        }



        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("userId") != null)
            {
                CartController obj = new CartController(_configuration);
                ViewBag.cart = obj.getUserCart(HttpContext.Session.GetString("userId") + "");

            }
            return View();
        }


        [HttpPost]
        public IActionResult Register(string fullName, string email, string password, string userType)
        {
        

            User user = new User();
            user.Username = fullName;
            user.Password = password;
            user.Email = email;
            user.usertype = userType;
            if (user.usertype == "الطالب")
            {
                _employeeData.AddEmployee(user);
               // return RedirectToAction("Index", "Home");
            }

            else
            {
                _employeeData.AddEmployee(user);
                //return RedirectToAction("SetupLibrary", "Library");
            }

            return RedirectToAction("Login", "Account");

        }

    }
}
