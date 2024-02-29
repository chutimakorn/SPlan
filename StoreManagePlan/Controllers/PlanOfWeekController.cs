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
        public static string _menu = "pof";
        private readonly IConfiguration _configuration;
        public PlanOfWeekController(StoreManagePlanContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(int Store,int Week)
        {
            ViewBag.store = Store;
            ViewBag.week = Week;
            var newWeek = Week;
            var newStore = Store;
            var weekList = _context.Week.ToList();
            var storeList = _context.Store.ToList();
            var storeTypeList = _context.StoreType.ToList();

            ViewBag.weekList = weekList;
            ViewBag.storeList = storeList;
            ViewBag.storeTypeList = storeTypeList;

            if (newWeek == 0)
            {
                newWeek = Convert.ToInt32(_configuration.GetSection("DefaultWeek").Value) + 1;
                ViewBag.week = Convert.ToInt16(_configuration.GetSection("DefaultWeek").Value) + 1;
            }
            else
            {
                ViewBag.week = newWeek;
            }

            if (newStore == 0)
            {
                var lastStore = storeList.FirstOrDefault();
                newStore = lastStore.id;
                ViewBag.store = lastStore.id;
            }
            else
            {
                ViewBag.store = newStore;
            }





            List<PlanDetail> planSpoke = await _context.PlanDetail
                    .Include(m => m.item)
                    .Join(_context.StoreRelation,
                        post => post.store_id,
                        meta => meta.store_spoke_id,
                        (post, meta) => new { Post = post, Meta = meta })
                    .Where(m => (m.Meta.store_hub_id == newStore) && m.Post.week_no == newWeek)
                    .Select(m => m.Post)
            .ToListAsync();


            List<PlanDetail> planHub = await _context.PlanDetail
                    .Include(m => m.item)
                    .Where(m => (m.store_id == newStore) && m.week_no == newWeek)
                    .Select(m => m)
            .ToListAsync();


            List<PlanDetail> combinedList = planHub.Concat(planSpoke).ToList();

            List<PlanDetailModel> resultList = combinedList
                    .GroupBy(pd => new { pd.item.sku_code, pd.item.sku_name })
                    .Select(group => new PlanDetailModel
                    {
                        //Key = new { group.Key.week_no, group.Key.store_id, group.Key.sku_id, group.Key.sku_name },
                        //PlanDetails = group.ToList(),
                        sku_code = group.Key.sku_code,
                        sku_name = group.Key.sku_name,
                        plan_mon = group.Sum(pd => pd.plan_mon),
                        plan_tues = group.Sum(pd => pd.plan_tues),
                        plan_wed = group.Sum(pd => pd.plan_wed),
                        plan_thu = group.Sum(pd => pd.plan_thu),
                        plan_fri = group.Sum(pd => pd.plan_fri),
                        plan_sat = group.Sum(pd => pd.plan_sat),
                        plan_sun = group.Sum(pd => pd.plan_sun),
                    })
            .ToList();



            //List<PlanDetailModel> resultList = await _context.PlanDetail
            //        .Include(m => m.item)
            //        .Join(_context.StoreRelation,
            //            post => post.store_id,
            //            meta => meta.store_spoke_id,
            //            (post, meta) => new { Post = post, Meta = meta })
            //        .Where(m => (m.Meta.store_hub_id == newStore || m.Post.store_id == newStore) && m.Post.week_no == newWeek)
            //        .GroupBy(pd => new { pd.Post.item.sku_code, pd.Post.item.sku_name})
            //        .Select(group => new PlanDetailModel
            //        {
            //            //Key = new { group.Key.week_no, group.Key.store_id, group.Key.sku_id, group.Key.sku_name },
            //            //PlanDetails = group.ToList(),
            //            sku_code = group.Key.sku_code,
            //            sku_name = group.Key.sku_name,
            //            plan_mon = group.Sum(pd => pd.Post.plan_mon),
            //            plan_tues = group.Sum(pd => pd.Post.plan_tues),
            //            plan_wed = group.Sum(pd => pd.Post.plan_wed),
            //            plan_thu = group.Sum(pd => pd.Post.plan_thu),
            //            plan_fri = group.Sum(pd => pd.Post.plan_fri),
            //            plan_sat = group.Sum(pd => pd.Post.plan_sat),
            //            plan_sun = group.Sum(pd => pd.Post.plan_sun),
            //        })
            //.ToListAsync();


            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.menu = "pof";
            return View(resultList);
        }
    }
}
