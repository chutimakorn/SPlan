using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using StoreManagePlan.Data;
using StoreManagePlan.Models;
using StoreManagePlan.Repository;
using NuGet.Packaging.Core;
using System.Text.Json;
using Elfie.Serialization;
using System.Globalization;
using System.Xml.Linq;
using System.Collections;

namespace StoreManagePlan.Controllers
{
    public class IbtInController : Controller
    {
        private readonly StoreManagePlanContext _context;
        public static string _menu = "ibti";
        private readonly IConfiguration _configuration;
        private readonly IUtility _utility;
        public IbtInController(StoreManagePlanContext context, IConfiguration configuration, IUtility utility)
        {
            _context = context;
            _configuration = configuration;
            _utility = utility;
        }
        public IActionResult Index(int Store, int Day, int Week)
        {

            ViewBag.store = Store;
            ViewBag.week = Week;
            ViewBag.day = Day;
            var newStore = Store;

            var weekList = _context.Week.Where(w => w.status == 2).ToList();
            var storeList = _context.Store.Include(m => m.store_type).Where(s => s.store_type.store_type_name == "Spoke").ToList();
            //var spokeList = _context.StoreRelation.Include(h => h.StoreHub).Include(s => s.StoreSpoke).Where(h => h.StoreHub.id == Store).Select(s => s.StoreSpoke).ToList();

            var planactual = _context.PlanActually
                .Where(s => s.store_id == Store && s.week_no == Week && s.day_of_week == Day)
                .Include(i => i.item)
                .AsEnumerable()
                //.GroupBy(s => new { s.item, s.sku_id, s.week_no, s.day_of_week })
                //.Select(group => new PlanActually {
                //    sku_id = group.Key.sku_id,
                //    week_no = group.Key.week_no,
                //    day_of_week = group.Key.day_of_week,
                //    plan_actually = group.Sum(pd => pd.plan_actually),
                //    plan_value = group.Sum(pd => pd.plan_value),
                //    item = group.Key.item,
                //    approve = group.OrderBy(p => p.some_order_field).Select(p => p.approve).FirstOrDefault()
                //})
                .ToList();

            ViewBag.storeList = storeList;
            ViewBag.weeklist = weekList;
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.menu = _menu;
            return View(planactual);
        }
        public IActionResult Edit(int Store, int Day, int Week, int SkuId)
        {
            ViewBag.store = Store;
            ViewBag.week = Week;
            ViewBag.day = Day;
            ViewBag.sku = SkuId;

            ViewBag.reasonhight = _context.Reason.Where(m => m.menu == "ibt" && m.type == 1).ToList();
            ViewBag.resonlow = _context.Reason.Where(m => m.menu == "ibt" && m.type == 0).ToList();
            ViewBag.reson = _context.Reason.Where(m => m.menu == "ibt").ToList();

            var spokeList = _context.StoreRelation.Include(h => h.StoreHub).Include(s => s.StoreSpoke).Where(h => h.StoreHub.id == Store).Select(s => s.StoreSpoke.id).ToList();

            var planactual = _context.PlanActually
                .Where(s => s.store_id == Store && s.week_no == Week && s.day_of_week == Day && s.sku_id == SkuId)
                .Include(i => i.item)
                .Include(i => i.store)
                .AsEnumerable()
                .ToList();
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.menu = _menu;
            return View(planactual);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitAll(string planActuals)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<List<PlanActually>>(planActuals);

                foreach (var n in jsonData)
                {
                    var temp = await _context.PlanActually
                        .Where(s => s.sku_id == n.sku_id && s.week_no == n.week_no && s.store_id == n.store_id
                        && s.day_of_week == n.day_of_week).FirstOrDefaultAsync();
                    temp.plan_actually = n.plan_actually;
                    temp.reason_id = n.reason_id;
                }

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return Json(new { success = false, massage = ex.Message });
            }

            return Json(new { success = true, massage = "บันทีกสำเร็จ" });
        }

        public async Task<IActionResult> Approve(int Spoke, int Week, int Day)
        {
            try
            {
                var temp = await _context.PlanActually
                        .Where(s => s.store_id == Spoke && s.week_no == Week && s.day_of_week == Day).ToListAsync();
                foreach (var item in temp)
                {
                    item.approve = 1;
                }
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return Json(new { success = false, massage = ex.Message });
            }

            return Json(new { success = true, massage = "บันทีกสำเร็จ" });
        }
    }
}
