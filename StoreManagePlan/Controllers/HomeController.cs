using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using StoreManagePlan.Models;
using System.Diagnostics;


namespace StoreManagePlan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.menu = "home";
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string user,string password)
        {

            var role = 0;
            if ( user == "admin" && password == "1234") 
            {

                role = 1;
            }
            else if(user == "planner" && password == "0000")
            {
                role = 2;
            }
            else if (user == "storehub" && password == "1111")
            {
                role = 3;
            }
            else if (user == "storespoke" && password == "2222")
            {
                role = 4;
            }
            else
            {

                ViewBag.alert = "user or password art not correct!";
                return View();
            }

           
          
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.Add(TimeSpan.FromDays(30)), // ??????????????????????? Cookie
                HttpOnly = true,
                IsEssential = true,
            };

            HttpContext.Response.Cookies.Append("Role", role.ToString(), cookieOptions);

            return RedirectToAction("Index", "Bom");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
