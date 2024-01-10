using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StoreManagePlan.Controllers
{
    public class ProductPlanReviewController : Controller
    {
        // GET: ProductPlanReviewController
        public ActionResult Index()
        {
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies.TryGetValue("Role", out string roleValue));
            return View();
        }

        // GET: ProductPlanReviewController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductPlanReviewController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductPlanReviewController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductPlanReviewController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductPlanReviewController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductPlanReviewController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductPlanReviewController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
