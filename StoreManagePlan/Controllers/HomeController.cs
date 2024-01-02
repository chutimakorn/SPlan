using Microsoft.AspNetCore.Mvc;
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
            else if(user == "user" && password == "0000")
            {
                role = 2;
            }
            else
            {

                ViewBag.alert = "user or password art not correct!";
                return View();
            }

            HttpContext.Session.SetInt32("Role", role);


            return RedirectToAction("Index");
          
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
