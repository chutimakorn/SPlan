using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Elfie.Serialization;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using StoreManagePlan.Data;
using StoreManagePlan.Models;
using StoreManagePlan.Repository;
using static StoreManagePlan.Controllers.PlanActuallyController;

namespace StoreManagePlan.Controllers
{
    public class PlanComplianceController : Controller
    {
        private readonly StoreManagePlanContext _context;
        IUtility _utility;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public static string _menu = "ppc";
        private readonly IConfiguration _configuration;

        public PlanComplianceController(StoreManagePlanContext context, IUtility utility, IConfiguration configuration)
        {
            _context = context;
            this._utility = utility;
            _configuration = configuration;
        }

        // GET: PlanCompliance
        public async Task<IActionResult> Index(int week)
        {
            ViewBag.menu = _menu;
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.week = week;
            ViewBag.weekMaster = _context.Week.ToList();
            ViewBag.reason = _context.Reason.ToList();
            List<PlanActually> actuallyPlan = new List<PlanActually>();
            if(week != 0)
            {
                actuallyPlan = await _context.PlanActually.Include(p => p.item).Include(p => p.store).ThenInclude(m => m.store_type).Include(p => p.week).Where(m => m.week_no == week).OrderBy(m => m.day_of_week).ToListAsync();
            }
         
           

         
            return View(actuallyPlan);
        }

        public IActionResult ExportToExcel(int id)
        {
            var data = _context.PlanActually.Include(p => p.item).Include(p => p.store).ThenInclude(m => m.store_type).Include(p => p.week).Where(m => m.week_no == id).OrderBy(m =>  m.day_of_week).ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header

                worksheet.Cells[1, 1].Value = "period week";
                worksheet.Cells[1, 2].Value = "finished date";
                worksheet.Cells[1, 3].Value = "plan day of week";
                worksheet.Cells[1, 4].Value = "sku";
                worksheet.Cells[1, 5].Value = "description";
                worksheet.Cells[1, 6].Value = "store";
                worksheet.Cells[1, 7].Value = "store name";
                worksheet.Cells[1, 8].Value = "shift";
                worksheet.Cells[1, 9].Value = "plan qty";
                worksheet.Cells[1, 10].Value = "actually qty";
                worksheet.Cells[1, 11].Value = "reason";
                // Add more columns as needed

                // Data
                for (var i = 0; i < data.Count; i++)
                {


                    var reason = _context.Reason.Where(n => n.id == data[i].reason_id).SingleOrDefault();
                    var reasonText = "";
                    var dayText = "";
                    var cycleText = "";
                    if (reason != null)
                    {
                        reasonText = reason.reason;
                    }

                    if(data[i].day_of_week == 1)
                    {
                        dayText = "Monday";
                    }
                    else if(data[i].day_of_week == 2)
                    {
                        dayText = "Tuesday";
                    }
                    else if(data[i].day_of_week == 3)
                    {
                        dayText = "Wednesday";
                    } 
                    else if(data[i].day_of_week == 4)
                    {
                        dayText = "Thuesday";
                    } 
                    else if(data[i].day_of_week == 5)
                    {
                        dayText = "Friday";
                    } 
                    else if(data[i].day_of_week == 6)
                    {
                        dayText = "Saturday";
                    }
                    else 
                    {
                        dayText = "Sunday";
                    }

                    if (data[0].store.store_type.store_type_name.ToLower() == "hub")
                    {
                        cycleText = "เช้า";
                    }
                    else
                    {
                        cycleText = "บ่าย";
                    }



                    worksheet.Cells[i + 2, 1].Value = _utility.ConvertDate(data[i].week.start_date) + " - " + _utility.ConvertDate(data[i].week.end_date);
                    worksheet.Cells[i + 2, 2].Value = data[i].week.start_date;
                    worksheet.Cells[i + 2, 3].Value = dayText;
                    worksheet.Cells[i + 2, 4].Value = data[i].item.sku_code;
                    worksheet.Cells[i + 2, 5].Value = data[i].item.sku_name;
                    worksheet.Cells[i + 2, 6].Value = data[i].store.store_code;
                    worksheet.Cells[i + 2, 7].Value = data[i].store.store_name;
                    worksheet.Cells[i + 2, 8].Value = cycleText;
                    worksheet.Cells[i + 2, 9].Value = data[i].plan_value;
                    worksheet.Cells[i + 2, 10].Value = data[i].plan_actually;
                    worksheet.Cells[i + 2, 11].Value = @reasonText;
                    // Add more columns as needed
                }

                package.Save(); // Save the Excel package
            }

            stream.Position = 0;

            // Set the content type and file name
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plan_Compliance.xlsx");
        }
    }
}
