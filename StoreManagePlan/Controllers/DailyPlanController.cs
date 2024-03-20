using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagePlan.Data;
using StoreManagePlan.Models;
using StoreManagePlan.Repository;

namespace StoreManagePlan.Controllers
{
    public class DailyPlanController : Controller
    {
        private readonly StoreManagePlanContext _context;
        IUtility _utility;

        private readonly IWebHostEnvironment _hostingEnvironment;
        public static string _menu = "landing";
        private readonly IConfiguration _configuration;

        public DailyPlanController(StoreManagePlanContext context, IUtility utility, IConfiguration configuration) {
            _context = context;
            this._utility = utility;
            _configuration = configuration;
        }

        // GET: DailyPlanController
        public async Task<IActionResult> Index(int weekNo, int storeId,int DayNo,int TabNo)
        {
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.menu = _menu;
            ViewBag.storeId = storeId;
  
            ViewBag.store = _context.Store.Include(m=> m.store_type).Where( n=> n.store_type.store_type_name == "Hub").ToList();
            ViewBag.weekMaster = _context.Week.ToList();
            ViewBag.day = DayNo;
            int week = weekNo;
            if (week == 0)
            {
                week = Convert.ToInt16(_configuration.GetSection("DefaultWeek").Value);
                ViewBag.weekNo = Convert.ToInt16(_configuration.GetSection("DefaultWeek").Value);
            }
            else
            {
                ViewBag.weekNo = week;
            }

            if (TabNo == 0)
            {
                ViewBag.tab_product = "active";
            }
            else
            {
                ViewBag.tab_ingredient = "active";
            }

            List<DailyProductModel> products = new List<DailyProductModel>();

            products = this.GetDailyProduct(week,storeId, DayNo);
            //products = this.GetDailyProduct(3, 52, 1);


            return View(products);
        }

        // GET: DailyPlanController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DailyPlanController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DailyPlanController/Create
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

        // GET: DailyPlanController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DailyPlanController/Edit/5
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

        // GET: DailyPlanController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DailyPlanController/Delete/5
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

        private  List<DailyProductModel> GetDailyProduct(int week, int store, int day)
        {
            List<DailyProductModel> dailyProductModels = new List<DailyProductModel>();

            //var productTemp = new DailyProductModel();

            var spoke = _context.StoreRelation.Include(n => n.StoreSpoke).Where(m => m.store_hub_id == store).Select(s => s.StoreSpoke.id).ToList();
            var listHub = _context.PlanDetail.Where(m => m.week_no == week && m.store_id == store)
            .GroupBy(pd => new { pd.item.sku_code, pd.item.sku_name })
            .Select(group => new PlanDetailModel
            {
                sku_code = group.Key.sku_code,
                sku_name = group.Key.sku_name,
                plan_mon = group.Sum(pd => pd.plan_mon),
                plan_tues = group.Sum(pd => pd.plan_tues),
                plan_wed = group.Sum(pd => pd.plan_wed),
                plan_thu = group.Sum(pd => pd.plan_thu),
                plan_fri = group.Sum(pd => pd.plan_fri),
                plan_sat = group.Sum(pd => pd.plan_sat),
                plan_sun = group.Sum(pd => pd.plan_sun),
                type = "hub"
            })
            .ToList();

            var listSpoke = _context.PlanDetail.Where(m => m.week_no == week && spoke.Contains(m.store_id))
            .GroupBy(pd => new { pd.item.sku_code, pd.item.sku_name })
            .Select(group => new PlanDetailModel
            {
                sku_code = group.Key.sku_code,
                sku_name = group.Key.sku_name,
                plan_mon = group.Sum(pd => pd.plan_mon),
                plan_tues = group.Sum(pd => pd.plan_tues),
                plan_wed = group.Sum(pd => pd.plan_wed),
                plan_thu = group.Sum(pd => pd.plan_thu),
                plan_fri = group.Sum(pd => pd.plan_fri),
                plan_sat = group.Sum(pd => pd.plan_sat),
                plan_sun = group.Sum(pd => pd.plan_sun),
                type = "spoke"
            })
            .ToList();

            var hubSkuNames = listHub.Select(t => t.sku_name).Where(s => s != null).ToList();
            var spokeSkuNames = listSpoke.Select(t => t.sku_name).Where(s => s != null).ToList();

            var comb = hubSkuNames.Union(spokeSkuNames);

            if (comb != null)
            {
                switch (day)
                {
                    case 1:
                        foreach (var item in comb)
                        {
                            var productTemp = new DailyProductModel();
                            productTemp.ProductName = item;
                            productTemp.HubAmt = listHub.Where(w => w.sku_name == item).Sum(s => s.plan_mon);
                            productTemp.SpokeAmt = listSpoke.Where(w => w.sku_name == item).Sum(s => s.plan_mon);
                            productTemp.TotalAmt = productTemp.HubAmt + productTemp.SpokeAmt;
                            dailyProductModels.Add(productTemp);
                        }
                        break;
                    case 2:
                        foreach (var item in comb)
                        {
                            var productTemp2 = new DailyProductModel();
                            productTemp2.ProductName = item;
                            productTemp2.HubAmt = listHub.Where(w => w.sku_name == item).Sum(s => s.plan_tues);
                            productTemp2.SpokeAmt = listSpoke.Where(w => w.sku_name == item).Sum(s => s.plan_tues);
                            productTemp2.TotalAmt = productTemp2.HubAmt + productTemp2.SpokeAmt;
                            dailyProductModels.Add(productTemp2);
                        }
                        break;
                    case 3:
                        foreach (var item in comb)
                        {
                            var productTemp3 = new DailyProductModel();
                            productTemp3.ProductName = item;
                            productTemp3.HubAmt = listHub.Where(w => w.sku_name == item).Sum(s => s.plan_wed);
                            productTemp3.SpokeAmt = listSpoke.Where(w => w.sku_name == item).Sum(s => s.plan_wed);
                            productTemp3.TotalAmt = productTemp3.HubAmt + productTemp3.SpokeAmt;
                            dailyProductModels.Add(productTemp3);
                        }
                        
                        break;
                    case 4:
                        foreach (var item in comb)
                        {
                            var productTemp4 = new DailyProductModel();
                            productTemp4.ProductName = item;
                            productTemp4.HubAmt = listHub.Where(w => w.sku_name == item).Sum(s => s.plan_thu);
                            productTemp4.SpokeAmt = listSpoke.Where(w => w.sku_name == item).Sum(s => s.plan_thu);
                            productTemp4.TotalAmt = productTemp4.HubAmt + productTemp4.SpokeAmt;
                            dailyProductModels.Add(productTemp4);
                        }
                        
                        break;
                    case 5:
                        foreach (var item in comb)
                        {
                            var productTemp5 = new DailyProductModel();
                            productTemp5.ProductName = item;
                            productTemp5.HubAmt = listHub.Where(w => w.sku_name == item).Sum(s => s.plan_fri);
                            productTemp5.SpokeAmt = listSpoke.Where(w => w.sku_name == item).Sum(s => s.plan_fri);
                            productTemp5.TotalAmt = productTemp5.HubAmt + productTemp5.SpokeAmt;
                            dailyProductModels.Add(productTemp5);
                        }

                        break;
                    case 6:
                        foreach (var item in comb)
                        {
                            var productTemp6 = new DailyProductModel();
                            productTemp6.ProductName = item;
                            productTemp6.HubAmt = listHub.Where(w => w.sku_name == item).Sum(s => s.plan_sat);
                            productTemp6.SpokeAmt = listSpoke.Where(w => w.sku_name == item).Sum(s => s.plan_sat);
                            productTemp6.TotalAmt = productTemp6.HubAmt + productTemp6.SpokeAmt;
                            dailyProductModels.Add(productTemp6);
                        }

                        break;
                    case 7:

                        foreach (var item in comb)
                        {
                            var productTemp7 = new DailyProductModel();
                            productTemp7.ProductName = item;
                            productTemp7.HubAmt = listHub.Where(w => w.sku_name == item).Sum(s => s.plan_sun);
                            productTemp7.SpokeAmt = listSpoke.Where(w => w.sku_name == item).Sum(s => s.plan_sun);
                            productTemp7.TotalAmt = productTemp7.HubAmt + productTemp7.SpokeAmt;
                            dailyProductModels.Add(productTemp7);
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }

            return  dailyProductModels;
        }
    }
}
