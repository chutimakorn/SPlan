using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
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
            ViewBag.tabNo = TabNo;
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
                ViewBag.tab_product_active = "active show";
            }
            else
            {
                ViewBag.tab_ingredient = "active";
                ViewBag.tab_ingredient_active = "active show";
            }

            List<DailyProductModel> products = new List<DailyProductModel>();

            products = this.GetDailyProduct(week,storeId, DayNo);

            List<Bom> boms = new List<Bom>();

            foreach (var item in products)
            { 
                var bomList = _context.Bom.Where(b=> b.sku_id == item.SkuId)
                    .Select(s=> 
                    new Bom { 
                        ingredient_sku = s.ingredient_sku,
                        ingredient_name = s.ingredient_name,
                        weight_uom = s.weight_uom,
                        weight_hub = s.weight_hub * item.TotalAmt,
                    })
                    .ToList();
                boms.AddRange(bomList);
            }

            boms = boms.GroupBy(pd => new {pd.ingredient_sku, pd.ingredient_name,pd.weight_uom })
            .Select(group => new Bom
            {
                ingredient_sku = group.Key.ingredient_sku,
                ingredient_name = group.Key.ingredient_name,
                weight_uom = group.Key.weight_uom,
                weight_hub = group.Sum(pd => pd.weight_hub),
            })
            .ToList();

            ViewBag.boms = boms;

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
            var listHub = _context.PlanDetail.Where(m => m.week_no == week && m.store_id == store && m.approve == true)
            .GroupBy(pd => new { pd.item.sku_code, pd.sku_id, pd.item.sku_name })
            .Select(group => new PlanDetailModel
            {
                sku_id = group.Key.sku_id,
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

            var listSpoke = _context.PlanDetail.Where(m => m.week_no == week && spoke.Contains(m.store_id) && m.approve == true)
            .GroupBy(pd => new { pd.item.sku_code, pd.sku_id, pd.item.sku_name })
            .Select(group => new PlanDetailModel
            {
                sku_id = group.Key.sku_id,
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

            var uomList = _context.Bom.Select(s => new { s.sku_id, s.batch_uom }).ToList();

            var hubSkuNames = listHub.Select(t => new { t.sku_name , t.sku_id }).Where(s => s != null).ToList();
            var spokeSkuNames = listSpoke.Select(t => new { t.sku_name, t.sku_id }).Where(s => s != null).ToList();

            var comb = hubSkuNames.Union(spokeSkuNames);




            if (comb != null)
            {
                switch (day)
                {
                    case 1:
                        foreach (var item in comb)
                        {
                            var productTemp = new DailyProductModel();
                            productTemp.SkuId = item.sku_id;
                            productTemp.ProductName = item.sku_name;

                            productTemp.Unit = uomList.Where(s => s.sku_id == item.sku_id).Select(c => c.batch_uom).FirstOrDefault();

                            var planActualHub = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 1 && s.store_id == store).ToList();
                            var planActualSpoke = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 1 && spoke.Contains(s.store_id)).ToList();

                            if (planActualHub.Count() > 0)
                            {
                                productTemp.HubAmt = planActualHub.Sum(s => s.plan_actually);
                                productTemp.SpokeAmt = planActualSpoke.Sum(s => s.plan_actually);
                                productTemp.TotalAmt = planActualHub.Sum(s => s.plan_actually) + planActualSpoke.Sum(s => s.plan_actually);
                            }
                            else
                            {
                                productTemp.HubAmt = listHub.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_mon);
                                productTemp.SpokeAmt = listSpoke.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_mon);
                                productTemp.TotalAmt = productTemp.HubAmt + productTemp.SpokeAmt;
                            }

                            dailyProductModels.Add(productTemp);
                        }
                        break;
                    case 2:
                        foreach (var item in comb)
                        {
                            var productTemp2 = new DailyProductModel();
                            productTemp2.SkuId = item.sku_id;
                            productTemp2.ProductName = item.sku_name;
                            productTemp2.Unit = uomList.Where(s => s.sku_id == item.sku_id).Select(c => c.batch_uom).FirstOrDefault();

                            var planActualHub = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 2 && s.store_id == store).ToList();
                            var planActualSpoke = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 2 && spoke.Contains(s.store_id)).ToList();

                            if (planActualHub.Count() > 0)
                            {
                                productTemp2.HubAmt = planActualHub.Sum(s => s.plan_actually);
                                productTemp2.SpokeAmt = planActualSpoke.Sum(s => s.plan_actually);
                                productTemp2.TotalAmt = planActualHub.Sum(s => s.plan_actually) + planActualSpoke.Sum(s => s.plan_actually);
                            }
                            else {
                                productTemp2.HubAmt = listHub.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_tues);
                                productTemp2.SpokeAmt = listSpoke.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_tues);
                                productTemp2.TotalAmt = productTemp2.HubAmt + productTemp2.SpokeAmt;
                            }

                            dailyProductModels.Add(productTemp2);
                        }
                        break;
                    case 3:
                        foreach (var item in comb)
                        {
                            var productTemp3 = new DailyProductModel();
                            productTemp3.SkuId = item.sku_id;
                            productTemp3.ProductName = item.sku_name;

                            productTemp3.Unit = uomList.Where(s => s.sku_id == item.sku_id).Select(c => c.batch_uom).FirstOrDefault();

                            var planActualHub = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 3 && s.store_id == store).ToList();
                            var planActualSpoke = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 3 && spoke.Contains(s.store_id)).ToList();

                            if (planActualHub.Count() > 0)
                            {
                                productTemp3.HubAmt = planActualHub.Sum(s => s.plan_actually);
                                productTemp3.SpokeAmt = planActualSpoke.Sum(s => s.plan_actually);
                                productTemp3.TotalAmt = planActualHub.Sum(s => s.plan_actually) + planActualSpoke.Sum(s => s.plan_actually);
                            }
                            else {
                                productTemp3.HubAmt = listHub.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_wed);
                                productTemp3.SpokeAmt = listSpoke.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_wed);
                                productTemp3.TotalAmt = productTemp3.HubAmt + productTemp3.SpokeAmt;
                            }

                            dailyProductModels.Add(productTemp3);
                        }
                        
                        break;
                    case 4:
                        foreach (var item in comb)
                        {
                            var productTemp4 = new DailyProductModel();
                            productTemp4.SkuId = item.sku_id;
                            productTemp4.ProductName = item.sku_name;

                            productTemp4.Unit = uomList.Where(s => s.sku_id == item.sku_id).Select(c => c.batch_uom).FirstOrDefault();

                            var planActualHub = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 4 && s.store_id == store).ToList();
                            var planActualSpoke = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 4 && spoke.Contains(s.store_id)).ToList();

                            if (planActualHub.Count() > 0)
                            {
                                productTemp4.HubAmt = planActualHub.Sum(s => s.plan_actually);
                                productTemp4.SpokeAmt = planActualSpoke.Sum(s => s.plan_actually);
                                productTemp4.TotalAmt = planActualHub.Sum(s => s.plan_actually) + planActualSpoke.Sum(s => s.plan_actually);
                            }
                            else
                            {
                                productTemp4.HubAmt = listHub.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_thu);
                                productTemp4.SpokeAmt = listSpoke.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_thu);
                                productTemp4.TotalAmt = productTemp4.HubAmt + productTemp4.SpokeAmt;
                            }

                            dailyProductModels.Add(productTemp4);
                        }
                        
                        break;
                    case 5:
                        foreach (var item in comb)
                        {
                            var productTemp5 = new DailyProductModel();
                            productTemp5.SkuId = item.sku_id;
                            productTemp5.ProductName = item.sku_name;

                            productTemp5.Unit = uomList.Where(s => s.sku_id == item.sku_id).Select(c => c.batch_uom).FirstOrDefault();

                            var planActualHub = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 5 && s.store_id == store).ToList();
                            var planActualSpoke = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 5 && spoke.Contains(s.store_id)).ToList();

                            if (planActualHub.Count() > 0)
                            {
                                productTemp5.HubAmt = planActualHub.Sum(s => s.plan_actually);
                                productTemp5.SpokeAmt = planActualSpoke.Sum(s => s.plan_actually);
                                productTemp5.TotalAmt = planActualHub.Sum(s => s.plan_actually) + planActualSpoke.Sum(s => s.plan_actually);
                            }
                            else
                            {
                                productTemp5.HubAmt = listHub.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_fri);
                                productTemp5.SpokeAmt = listSpoke.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_fri);
                                productTemp5.TotalAmt = productTemp5.HubAmt + productTemp5.SpokeAmt;
                            }

                            dailyProductModels.Add(productTemp5);
                        }

                        break;
                    case 6:
                        foreach (var item in comb)
                        {
                            var productTemp6 = new DailyProductModel();
                            productTemp6.SkuId = item.sku_id;
                            productTemp6.ProductName = item.sku_name;
                            productTemp6.Unit = uomList.Where(s => s.sku_id == item.sku_id).Select(c => c.batch_uom).FirstOrDefault();

                            var planActualHub = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 6 && s.store_id == store).ToList();
                            var planActualSpoke = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 6 && spoke.Contains(s.store_id)).ToList();

                            if (planActualHub.Count() > 0)
                            {
                                productTemp6.HubAmt = planActualHub.Sum(s => s.plan_actually);
                                productTemp6.SpokeAmt = planActualSpoke.Sum(s => s.plan_actually);
                                productTemp6.TotalAmt = planActualHub.Sum(s => s.plan_actually) + planActualSpoke.Sum(s => s.plan_actually);
                            }
                            else {
                                productTemp6.HubAmt = listHub.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_sat);
                                productTemp6.SpokeAmt = listSpoke.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_sat);
                                productTemp6.TotalAmt = productTemp6.HubAmt + productTemp6.SpokeAmt;
                            }

                            dailyProductModels.Add(productTemp6);
                        }

                        break;
                    case 7:

                        foreach (var item in comb)
                        {
                            var productTemp7 = new DailyProductModel();
                            productTemp7.SkuId = item.sku_id;
                            productTemp7.ProductName = item.sku_name;

                            productTemp7.Unit = uomList.Where(s => s.sku_id == item.sku_id).Select(c => c.batch_uom).FirstOrDefault();

                            var planActualHub = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 7 && s.store_id == store).ToList();
                            var planActualSpoke = _context.PlanActually.Where(s => s.sku_id == item.sku_id && s.week_no == week && s.day_of_week == 7 && spoke.Contains(s.store_id)).ToList();

                            if (planActualHub.Count() > 0)
                            {
                                productTemp7.HubAmt = planActualHub.Sum(s => s.plan_actually);
                                productTemp7.SpokeAmt = planActualSpoke.Sum(s => s.plan_actually);
                                productTemp7.TotalAmt = planActualHub.Sum(s => s.plan_actually) + planActualSpoke.Sum(s => s.plan_actually);
                            }
                            else
                            {
                                productTemp7.HubAmt = listHub.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_sun);
                                productTemp7.SpokeAmt = listSpoke.Where(w => w.sku_id == item.sku_id).Sum(s => s.plan_sun);
                                productTemp7.TotalAmt = productTemp7.HubAmt + productTemp7.SpokeAmt;
                            }

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
