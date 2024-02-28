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

namespace StoreManagePlan.Controllers
{
    public class PlanOfWeekController : Controller
    {
        private readonly StoreManagePlanContext _context;
        public static string _menu = "playOfWeek";
        public PlanOfWeekController(StoreManagePlanContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? StoreType,int? Store,int? Week)
        {

            var weekList = _context.Week.ToList();
            var storeList = _context.Store.ToList();
            var storeTypeList = _context.StoreType.ToList();

            ViewBag.weekList = weekList;
            ViewBag.storeList = storeList;
            ViewBag.storeTypeList = storeTypeList;

            IQueryable<PlanDetail> planDetaiQuery = _context.PlanDetail
                .Include(m => m.week)
                .Include(m => m.store);

            if (StoreType != null)
            {
                planDetaiQuery = planDetaiQuery.Where(m => m.store.type_id == StoreType);
            }

            if (Store != null)
            {
                planDetaiQuery = planDetaiQuery.Where(m => m.store_id == Store);
            }

            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.menu = "playOfWeek";
            return View(await planDetaiQuery.ToListAsync());
        }
    }
}
