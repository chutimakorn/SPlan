using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elfie.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using StoreManagePlan.Data;
using StoreManagePlan.Models;
using StoreManagePlan.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StoreManagePlan.Controllers
{
    public class PlanPerformanceController : Controller
    {
        private readonly StoreManagePlanContext _context;
        IUtility _utility;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public static string _menu = "pperf";
        private readonly IConfiguration _configuration;

        public PlanPerformanceController(StoreManagePlanContext context, IUtility utility, IConfiguration configuration)
        {
            _context = context;
            this._utility = utility;
            _configuration = configuration;
        }

        public class PlanPerformance
        {
            public string plan_week { get; set; }
            public string finished_date { get; set; }
            public string day_of_week { get; set; }
            public string sku_code { get; set; }
            public string sku_name { get; set; }
            public string store_code { get; set; }
            public string store_name { get; set; }
            public int normal_sale { get; set; }
            public int rtc_sale { get; set; }
            public double rtc_percent { get; set; }
            public int bad_qty { get; set; }
            public double bad_percent { get; set; }

        }

        // GET: PlanPerformance
        public async Task<IActionResult> Index(int week)
        {
            ViewBag.menu = _menu;
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.week = week;
            ViewBag.weekMaster = _context.Week.ToList();
            ViewBag.reason = _context.Reason.ToList();

            List<PlanPerformance> planPerformances = new List<PlanPerformance>();

            if (week != 0)
            {
                var saleHistoryList = await _context.SaleHistory.Where(m => m.week_no == week).ToListAsync();

                foreach (var saleHistory in saleHistoryList)
                {
                    var getWeek = _context.Week.Where(m => m.week_no == saleHistory.week_no).SingleOrDefault();

                    // วันที่ในรูปแบบ yyyymmdd
                    string dateString = getWeek.start_date;

                    // แปลงข้อมูลเป็น DateTime
                    DateTime dateTime = DateTime.ParseExact(dateString, "yyyyMMdd", null);

                    // ลบหนึ่งวัน
                    dateTime = dateTime.AddDays(-1);

                    var getStore = _context.Store.Where(m => m.store_code == saleHistory.store_code).SingleOrDefault();
                    var getSKu = _context.Item.Where(m => m.sku_code == saleHistory.sku_code).SingleOrDefault();




                    var dayofweek = "";
                    if (saleHistory.day_of_week == 1)
                    {
                        dayofweek = "Monday";
                    }
                    else if (saleHistory.day_of_week == 2)
                    {
                        dayofweek = "Tuesday";
                    }
                    else if (saleHistory.day_of_week == 3)
                    {
                        dayofweek = "Wednesday";
                    }
                    else if (saleHistory.day_of_week == 4)
                    {
                        dayofweek = "Thuesday";
                    }
                    else if (saleHistory.day_of_week == 5)
                    {
                        dayofweek = "Friday";
                    }
                    else if (saleHistory.day_of_week == 6)
                    {
                        dayofweek = "Saturday";
                    }
                    else
                    {
                        dayofweek = "Sunday";
                    }


                    PlanPerformance planPerformance = new PlanPerformance();
                    //planPerformance.plan_week = _utility.ConvertDate(getWeek.start_date) + "-" + _utility.ConvertDate(getWeek.end_date);
                    planPerformance.plan_week = "W"+getWeek.week_no;
                    planPerformance.finished_date = dateTime.ToString("yyyy-MM-dd");
                    planPerformance.day_of_week = dayofweek;
                    planPerformance.sku_code = getSKu != null? getSKu.sku_code:"";
                    planPerformance.sku_name = getSKu != null ? getSKu.sku_name : "";
                    planPerformance.store_code = getStore != null ? getStore.store_code : "";
                    planPerformance.store_name = getStore != null ? getStore.store_name : "";
                    planPerformance.normal_sale = saleHistory.qty_base;
                    planPerformance.rtc_sale = saleHistory.qty_rtc;
                    planPerformance.rtc_percent = saleHistory.rtc_percent;
                    planPerformance.bad_qty = saleHistory.qty_bad;
                    planPerformance.bad_percent = saleHistory.bad_percent;

                    planPerformances.Add(planPerformance);




                }
            }



            return View(planPerformances);
        }

        public IActionResult ExportToExcel(int id)
        {
            List<PlanPerformance> planPerformances = new List<PlanPerformance>();

            if (id != 0)
            {
                var saleHistoryList =  _context.SaleHistory.Where(m => m.week_no == id).ToList();

                foreach (var saleHistory in saleHistoryList)
                {
                    var getWeek = _context.Week.Where(m => m.week_no == saleHistory.week_no).SingleOrDefault();

                    // วันที่ในรูปแบบ yyyymmdd
                    string dateString = getWeek.start_date;

                    // แปลงข้อมูลเป็น DateTime
                    DateTime dateTime = DateTime.ParseExact(dateString, "yyyyMMdd", null);

                    // ลบหนึ่งวัน
                    dateTime = dateTime.AddDays(-1);

                    var getStore = _context.Store.Where(m => m.store_code == saleHistory.store_code).SingleOrDefault();
                    var getSKu = _context.Item.Where(m => m.sku_code == saleHistory.sku_code).SingleOrDefault();




                    var dayofweek = "";
                    if (saleHistory.day_of_week == 1)
                    {
                        dayofweek = "Monday";
                    }
                    else if (saleHistory.day_of_week == 2)
                    {
                        dayofweek = "Tuesday";
                    }
                    else if (saleHistory.day_of_week == 3)
                    {
                        dayofweek = "Wednesday";
                    }
                    else if (saleHistory.day_of_week == 4)
                    {
                        dayofweek = "Thuesday";
                    }
                    else if (saleHistory.day_of_week == 5)
                    {
                        dayofweek = "Friday";
                    }
                    else if (saleHistory.day_of_week == 6)
                    {
                        dayofweek = "Saturday";
                    }
                    else
                    {
                        dayofweek = "Sunday";
                    }


                    PlanPerformance planPerformance = new PlanPerformance();
                    //planPerformance.plan_week = _utility.ConvertDate(getWeek.start_date) + "-" + _utility.ConvertDate(getWeek.end_date);
                    planPerformance.plan_week = "W" + getWeek.week_no; planPerformance.finished_date = dateTime.ToString("yyyy-MM-dd");
                    planPerformance.day_of_week = dayofweek;
                    planPerformance.sku_code = getSKu != null ? getSKu.sku_code : "";
                    planPerformance.sku_name = getSKu != null ? getSKu.sku_name : "";
                    planPerformance.store_code = getStore != null ? getStore.store_code : "";
                    planPerformance.store_name = getStore != null ? getStore.store_name : "";
                    planPerformance.normal_sale = saleHistory.qty_base;
                    planPerformance.rtc_sale = saleHistory.qty_rtc;
                    planPerformance.rtc_percent = saleHistory.rtc_percent;
                    planPerformance.bad_qty = saleHistory.qty_bad;
                    planPerformance.bad_percent = saleHistory.bad_percent;

                    planPerformances.Add(planPerformance);




                }
            }

            var data = planPerformances.ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header

                worksheet.Cells[1, 1].Value = "plan week";
                worksheet.Cells[1, 2].Value = "finished date";
                worksheet.Cells[1, 3].Value = "plan day of week";
                worksheet.Cells[1, 4].Value = "sku";
                worksheet.Cells[1, 5].Value = "description";
                worksheet.Cells[1, 6].Value = "store";
                worksheet.Cells[1, 7].Value = "store name";
                worksheet.Cells[1, 8].Value = "normal sale";
                worksheet.Cells[1, 9].Value = "rtc sale";
                worksheet.Cells[1, 10].Value = "rtc percent";
                worksheet.Cells[1, 11].Value = "bad qty";
                worksheet.Cells[1, 12].Value = "bad percent";
                // Add more columns as needed

                // Data
                for (var i = 0; i < data.Count; i++)
                {


                    worksheet.Cells[i + 2, 1].Value = data[i].plan_week;
                    worksheet.Cells[i + 2, 2].Value = data[i].finished_date;
                    worksheet.Cells[i + 2, 3].Value = data[i].day_of_week;
                    worksheet.Cells[i + 2, 4].Value = data[i].sku_code;
                    worksheet.Cells[i + 2, 5].Value = data[i].sku_name;
                    worksheet.Cells[i + 2, 6].Value = data[i].store_code;
                    worksheet.Cells[i + 2, 7].Value = data[i].store_name;
                    worksheet.Cells[i + 2, 8].Value = data[i].normal_sale;
                    worksheet.Cells[i + 2, 9].Value = data[i].rtc_sale;
                    worksheet.Cells[i + 2, 10].Value = data[i].rtc_percent;
                    worksheet.Cells[i + 2, 11].Value = data[i].bad_qty;
                    worksheet.Cells[i + 2, 12].Value = data[i].bad_percent;
                    // Add more columns as needed
                }

                package.Save(); // Save the Excel package
            }

            stream.Position = 0;

            // Set the content type and file name
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plan_Performance.xlsx");
        }
    }
}
