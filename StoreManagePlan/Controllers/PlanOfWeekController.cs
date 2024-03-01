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
        private readonly IUtility _utility;
        public PlanOfWeekController(StoreManagePlanContext context, IConfiguration configuration,IUtility utility)
        {
            _context = context;
            _configuration = configuration;
            _utility = utility;
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

            var total = resultList
                .GroupBy(pd => 1) // Group all results into a single group
            .Select(group => new PlanDetailModel
            {
                sku_code = "Total", // You can use any identifier for the total row
                sku_name = "Total",
                plan_mon = group.Sum(pd => pd.plan_mon),
                plan_tues = group.Sum(pd => pd.plan_tues),
                plan_wed = group.Sum(pd => pd.plan_wed),
                plan_thu = group.Sum(pd => pd.plan_thu),
                plan_fri = group.Sum(pd => pd.plan_fri),
                plan_sat = group.Sum(pd => pd.plan_sat),
                plan_sun = group.Sum(pd => pd.plan_sun),
            })
            .ToList().FirstOrDefault();


            ViewBag.total = total;

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

        public IActionResult ExportToExcel(int Store, int Week)
        {

            List<PlanDetail> planSpoke =  _context.PlanDetail
                    .Include(m => m.item)
                    .Join(_context.StoreRelation,
                        post => post.store_id,
                        meta => meta.store_spoke_id,
                        (post, meta) => new { Post = post, Meta = meta })
                    .Where(m => (m.Meta.store_hub_id == Store) && m.Post.week_no == Week)
                    .Select(m => m.Post)
            .ToList();


            List<PlanDetail> planHub =  _context.PlanDetail
                    .Include(m => m.item)
                    .Where(m => (m.store_id == Store) && m.week_no == Week)
                    .Select(m => m)
            .ToList();


            List<PlanDetail> combinedList = planHub.Concat(planSpoke).ToList();

            List<PlanDetailModel> data = combinedList
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

            var total = data
                .GroupBy(pd => 1) // Group all results into a single group
            .Select(group => new PlanDetailModel
            {
                sku_code = "Total", // You can use any identifier for the total row
                sku_name = "Total",
                plan_mon = group.Sum(pd => pd.plan_mon),
                plan_tues = group.Sum(pd => pd.plan_tues),
                plan_wed = group.Sum(pd => pd.plan_wed),
                plan_thu = group.Sum(pd => pd.plan_thu),
                plan_fri = group.Sum(pd => pd.plan_fri),
                plan_sat = group.Sum(pd => pd.plan_sat),
                plan_sun = group.Sum(pd => pd.plan_sun),
            })
            .ToList().FirstOrDefault();

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
                worksheet.Cells[1, 1].Value = "Sku Code";
                worksheet.Cells[1, 2].Value = "Sku Name";
                worksheet.Cells[1, 3].Value = "Plan";
                worksheet.Cells[2, 3].Value = "Mon";
                worksheet.Cells[2, 4].Value = "Tue";
                worksheet.Cells[2, 5].Value = "Wed";
                worksheet.Cells[2, 6].Value = "Thu";
                worksheet.Cells[2, 7].Value = "Fri";
                worksheet.Cells[2, 8].Value = "Sat";
                worksheet.Cells[2, 9].Value = "Sun";
                // Add more columns as needed

                // Data
                for (var i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 3, 1].Value = data[i].sku_code;
                    worksheet.Cells[i + 3, 2].Value = data[i].sku_name;
                    worksheet.Cells[i + 3, 3].Value = data[i].plan_mon;
                    worksheet.Cells[i + 3, 4].Value = data[i].plan_tues;
                    worksheet.Cells[i + 3, 5].Value = data[i].plan_wed;
                    worksheet.Cells[i + 3, 6].Value = data[i].plan_thu;
                    worksheet.Cells[i + 3, 7].Value = data[i].plan_fri;
                    worksheet.Cells[i + 3, 8].Value = data[i].plan_sat;
                    worksheet.Cells[i + 3, 9].Value = data[i].plan_sun;
                    // Add more columns as needed
                }

                var cnt = data.Count;

                if (cnt > 0)
                {
                    //total
                    worksheet.Cells[cnt + 3, 1].Value = "Total";
                    worksheet.Cells[cnt + 3, 3].Value = total.plan_mon;
                    worksheet.Cells[cnt + 3, 4].Value = total.plan_tues;
                    worksheet.Cells[cnt + 3, 5].Value = total.plan_wed;
                    worksheet.Cells[cnt + 3, 6].Value = total.plan_thu;
                    worksheet.Cells[cnt + 3, 7].Value = total.plan_fri;
                    worksheet.Cells[cnt + 3, 8].Value = total.plan_sat;
                    worksheet.Cells[cnt + 3, 9].Value = total.plan_sun;
                }



                _utility.MergeRowspanHeaders(worksheet, 1, 1, 2, 1); // Merge from row 1 to row 2 in column 1
                _utility.MergeRowspanHeaders(worksheet, 1, 2, 2, 2);
                _utility.MergeColspanHeaders(worksheet, 1, 3, 1, 9); // Merge from column 2 to column 3 in row 1
                _utility.MergeColspanHeaders(worksheet, cnt + 3, 1, cnt + 3, 2);
                package.Save(); // Save the Excel package
            }

            stream.Position = 0;



            // Set the content type and file name
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PlanOfWeek_List.xlsx");
        }
    }
}
