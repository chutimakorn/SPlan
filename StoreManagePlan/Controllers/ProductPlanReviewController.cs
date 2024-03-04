using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using StoreManagePlan.Data;
using StoreManagePlan.Models;
using StoreManagePlan.Repository;
using System.Linq;

namespace StoreManagePlan.Controllers
{
    public class ProductPlanReviewController : Controller
    {

        IUtility _utility;
        private readonly StoreManagePlanContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public static string _menu = "ppr";
        private readonly IConfiguration _configuration;

        public ProductPlanReviewController(StoreManagePlanContext context, IUtility utility, IConfiguration configuration)
        {
            _context = context;
            this._utility = utility;
            _configuration = configuration;
        }


        // GET: ProductPlanReviewController
        public ActionResult Index(int weekNo, int storeId,int TabNo)
        {

            
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.menu = _menu;
            ViewBag.storeId = storeId;
            ViewBag.tabNo = TabNo;
            ViewBag.store = _context.Store.ToList();
            ViewBag.weekMaster = _context.Week.ToList();

            if (TabNo == 0)
            {
                ViewBag.summary = "active";
            }
            else
            {
                ViewBag.detail = "active";
            }

            if (weekNo == 0)
            {
                weekNo = Convert.ToInt16(_configuration.GetSection("DefaultWeek").Value) +1;
                ViewBag.weekNo = Convert.ToInt16(_configuration.GetSection("DefaultWeek").Value) +1;
            }
            else
            {
                ViewBag.weekNo = weekNo;
            }

            IQueryable<PlanDetail>? modelQuery = _context.PlanDetail.Include(m => m.store).ThenInclude(m => m.store_type).Include(m => m.item);
            if (storeId != 0)
            {
                modelQuery = modelQuery.Where(m => m.store_id == storeId);
            }

           

           

           


            var model = modelQuery.Where(m => m.week_no == weekNo).ToList();

           
            var plan = _context.PlanDetail.Include(m => m.store).ThenInclude(m => m.store_type).Include(m => m.item).Where(m => m.week_no == weekNo).ToList();

            ViewBag.hubQTY = 0;
            ViewBag.spokeQTY = 0;
            ViewBag.spokePST = 0;
            ViewBag.hubPST = 0;
            ViewBag.totalQTY = 0;
            ViewBag.totalPST = 0;

            if (plan.Count() != 0)
            {
                // เลือกข้อมูลสำหรับ week_no
                var weekThreeData = model.Where(m => m.week_no == weekNo).ToList();

                // หาผลรวมของข้อมูลทั้งหมดใน week_no นั้น
                var totalSum = weekThreeData.Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // หาผลรวมของข้อมูลที่ store_type เป็น "HUB"
                var hubSum = weekThreeData.Where(m => m.store.store_type.store_type_code == "3001")
                    .Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // หาผลรวมของข้อมูลที่ store_type เป็น "SPOKE"
                var spokeSum = weekThreeData.Where(m => m.store.store_type.store_type_code == "3002")
                    .Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // คำนวณเป็น percent
                var hubPercent = ((double)hubSum / totalSum * 100).ToString("N2");
                var spokePercent = ((double)spokeSum / totalSum * 100).ToString("N2");
                ViewBag.hubQTY = hubSum;
                ViewBag.spokeQTY = spokeSum;
                ViewBag.spokePST = spokePercent;
                ViewBag.hubPST = hubPercent;
                ViewBag.totalQTY = totalSum;
                ViewBag.totalPST = 100;
            }

            return View(model);
        }

        

        [HttpPost]
        public async Task<IActionResult>  Calculate(int weekNo,int storeId)
        {
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.menu = _menu;
            ViewBag.weekNo = weekNo;
            ViewBag.storeId = storeId;
            ViewBag.store = _context.Store.ToList();
            ViewBag.weekMaster = _context.Week.ToList();

            var date = Convert.ToInt32(_utility.CreateDate());
            var storeList = _context.Store
                                .Include(m => m.store_type)
                                .Where(m =>Convert.ToInt32(m.start_date) <= date && date <= Convert.ToInt32(m.end_date)).ToList();
            
            foreach (var store in storeList)
            {
                var itemFeatureList = _context.ItemFeature
                                              .Include(m => m.Store)
                                              .Include(m => m.Item)
                                              .Where(m => m.store_id == store.id).ToList();
                
                foreach (var itemFeature in itemFeatureList)
                {
                    PlanDetail planDetail = new PlanDetail();
                    planDetail.sku_id = itemFeature.item_id;
                    planDetail.store_id = store.id;
                    planDetail.week_no = weekNo;

                    for (int day = 1; day <= 7; day++)
                    {
                        var runRate = 0;
                        for (int week = weekNo - 6; week <= weekNo; week++)
                        {
                            var saleHistory =  _context.SaleHistory.Where(m => m.day_of_week == day && m.week_no == week && m.store_code == store.store_code && m.sku_code == itemFeature.Item.sku_code).FirstOrDefault();

                            if(saleHistory != null)
                            {
                                if(saleHistory.bad_percent > 20 && saleHistory.rtc_percent > 10)
                                {
                                    runRate = runRate + (saleHistory.qty_base - saleHistory.qty_rtc);

                                }
                                else if(saleHistory.bad_percent <= 5 && saleHistory.rtc_percent <= 10)
                                {
                                    runRate = runRate + (saleHistory.qty_base + ((saleHistory.qty_rtc*50)/100));
                                }
                                else
                                {
                                    runRate = runRate + saleHistory.qty_base;
                                }

                            }
                            else
                            {
                                runRate = runRate + itemFeature.default_feature;
                            }


                        }

                        //mon
                        if(day == 1)
                        {
                            var avgRunRate = runRate / 6;

                            if(avgRunRate > itemFeature.maximum_feature)
                            {
                                planDetail.plan_mon = itemFeature.maximum_feature;
                            }
                            else if(avgRunRate < itemFeature.minimum_feature)
                            {
                                planDetail.plan_mon = itemFeature.minimum_feature;
                            }
                            else
                            {
                                planDetail.plan_mon = avgRunRate;
                            }
                           
                        }

                        //tues
                        else if (day == 2)
                        {
                            var avgRunRate = runRate / 6;

                            if (avgRunRate > itemFeature.maximum_feature)
                            {
                                planDetail.plan_tues = itemFeature.maximum_feature;
                            }
                            else if (avgRunRate < itemFeature.minimum_feature)
                            {
                                planDetail.plan_tues = itemFeature.minimum_feature;
                            }
                            else
                            {
                                planDetail.plan_tues = avgRunRate;
                            }

                        }

                        //wedn
                        else if (day == 3)
                        {
                            var avgRunRate = runRate / 6;

                            if (avgRunRate > itemFeature.maximum_feature)
                            {
                                planDetail.plan_wed = itemFeature.maximum_feature;
                            }
                            else if (avgRunRate < itemFeature.minimum_feature)
                            {
                                planDetail.plan_wed = itemFeature.minimum_feature;
                            }
                            else
                            {
                                planDetail.plan_wed = avgRunRate;
                            }

                        }

                        //thu
                        else if (day == 4)
                        {
                            var avgRunRate = runRate / 6;

                            if (avgRunRate > itemFeature.maximum_feature)
                            {
                                planDetail.plan_thu = itemFeature.maximum_feature;
                            }
                            else if (avgRunRate < itemFeature.minimum_feature)
                            {
                                planDetail.plan_thu = itemFeature.minimum_feature;
                            }
                            else
                            {
                                planDetail.plan_thu = avgRunRate;
                            }

                        }

                        //fri
                        else if (day == 5)
                        {
                            var avgRunRate = runRate / 6;

                            if (avgRunRate > itemFeature.maximum_feature)
                            {
                                planDetail.plan_fri = itemFeature.maximum_feature;
                            }
                            else if (avgRunRate < itemFeature.minimum_feature)
                            {
                                planDetail.plan_fri = itemFeature.minimum_feature;
                            }
                            else
                            {
                                planDetail.plan_fri = avgRunRate;
                            }
                        }

                        //sat
                        else if (day == 6)
                        {
                            var avgRunRate = runRate / 6;

                            if (avgRunRate > itemFeature.maximum_feature)
                            {
                                planDetail.plan_sat = itemFeature.maximum_feature;
                            }
                            else if (avgRunRate < itemFeature.minimum_feature)
                            {
                                planDetail.plan_sat = itemFeature.minimum_feature;
                            }
                            else
                            {
                                planDetail.plan_sat = avgRunRate;
                            }

                        }

                        //sun
                        else
                        {
                            var avgRunRate = runRate / 6;

                            if (avgRunRate > itemFeature.maximum_feature)
                            {
                                planDetail.plan_sun = itemFeature.maximum_feature;
                            }
                            else if (avgRunRate < itemFeature.minimum_feature)
                            {
                                planDetail.plan_sun = itemFeature.minimum_feature;
                            }
                            else
                            {
                                planDetail.plan_sun = avgRunRate;
                            }

                        }
                    }

                    var planDetailOwn = _context.PlanDetail.Where(m => m.week_no == weekNo).FirstOrDefault();
                    if(planDetailOwn != null)
                    {
                        planDetailOwn.plan_mon = planDetail.plan_mon;
                        planDetailOwn.plan_tues = planDetail.plan_tues;
                        planDetailOwn.plan_wed = planDetail.plan_wed;
                        planDetailOwn.plan_thu = planDetail.plan_thu;
                        planDetailOwn.plan_fri = planDetail.plan_fri;
                        planDetailOwn.plan_sat = planDetail.plan_sat;
                        planDetailOwn.plan_sun = planDetail.plan_sun;
                        _context.Update(planDetailOwn);
                    }
                    else
                    {
                        _context.Add(planDetail);
                    }
                    
                }

            }


            await _context.SaveChangesAsync();


            IQueryable<PlanDetail>? modelQuery = _context.PlanDetail.Include(m => m.store).Include(m => m.item);
            if (storeId != 0)
            {
                modelQuery = modelQuery.Where(m => m.store_id == storeId);
            }

            var model = modelQuery.Where(m => m.week_no == weekNo).ToList();

            var plan = _context.PlanDetail.Include(m => m.store).ThenInclude(m => m.store_type).Include(m => m.item).Where(m => m.week_no == weekNo).ToList();

            ViewBag.hubQTY = 0;
            ViewBag.spokeQTY = 0;
            ViewBag.spokePST = 0;
            ViewBag.hubPST = 0;
            ViewBag.totalQTY = 0;
            ViewBag.totalPST = 0;

            if (plan.Count() != 0)
            {
                // เลือกข้อมูลสำหรับ week_no
                var weekThreeData = model.Where(m => m.week_no == weekNo).ToList();

                // หาผลรวมของข้อมูลทั้งหมดใน week_no นั้น
                var totalSum = weekThreeData.Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // หาผลรวมของข้อมูลที่ store_type เป็น "HUB"
                var hubSum = weekThreeData.Where(m => m.store.store_type.store_type_code == "3001")
                    .Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // หาผลรวมของข้อมูลที่ store_type เป็น "SPOKE"
                var spokeSum = weekThreeData.Where(m => m.store.store_type.store_type_code == "3002")
                    .Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // คำนวณเป็น percent
                var hubPercent = ((double)hubSum / totalSum * 100).ToString("N2");
                var spokePercent = ((double)spokeSum / totalSum * 100).ToString("N2");
                ViewBag.hubQTY = hubSum;
                ViewBag.spokeQTY = spokeSum;
                ViewBag.spokePST = spokePercent;
                ViewBag.hubPST = hubPercent;
                ViewBag.totalQTY = totalSum;
                ViewBag.totalPST = 100;
            }



            return View("Index", model);
        }


        [HttpPost]
        public async Task<IActionResult> Submit(int weekNo, int storeId)
        {


            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.menu = _menu;
            ViewBag.weekNo = weekNo;
            ViewBag.storeId = storeId;
            ViewBag.store = _context.Store.ToList();
            ViewBag.weekMaster = _context.Week.ToList();


            var weekMaster = _context.Week.Where(m => m.week_no == weekNo).SingleOrDefault();

            //submit == 1
            //approve == 2
            //reject == 3
            //default == 0 || null

            weekMaster.status = 1;

            _context.Update(weekMaster);

            await _context.SaveChangesAsync();

            IQueryable<PlanDetail>? modelQuery = _context.PlanDetail.Include(m => m.store).Include(m => m.item);
            if (storeId != 0)
            {
                modelQuery = modelQuery.Where(m => m.store_id == storeId);
            }
            var model = modelQuery.Where(m => m.week_no == weekNo).ToList();
            var plan = _context.PlanDetail.Include(m => m.store).ThenInclude(m => m.store_type).Include(m => m.item).Where(m => m.week_no == weekNo).ToList();

            ViewBag.hubQTY = 0;
            ViewBag.spokeQTY = 0;
            ViewBag.spokePST = 0;
            ViewBag.hubPST = 0;
            ViewBag.totalQTY = 0;
            ViewBag.totalPST = 0;

            if (plan.Count() != 0)
            {
                // เลือกข้อมูลสำหรับ week_no
                var weekThreeData = model.Where(m => m.week_no == weekNo).ToList();

                // หาผลรวมของข้อมูลทั้งหมดใน week_no นั้น
                var totalSum = weekThreeData.Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // หาผลรวมของข้อมูลที่ store_type เป็น "HUB"
                var hubSum = weekThreeData.Where(m => m.store.store_type.store_type_code == "3001")
                    .Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // หาผลรวมของข้อมูลที่ store_type เป็น "SPOKE"
                var spokeSum = weekThreeData.Where(m => m.store.store_type.store_type_code == "3002")
                    .Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // คำนวณเป็น percent
                var hubPercent = ((double)hubSum / totalSum * 100).ToString("N2");
                var spokePercent = ((double)spokeSum / totalSum * 100).ToString("N2");
                ViewBag.hubQTY = hubSum;
                ViewBag.spokeQTY = spokeSum;
                ViewBag.spokePST = spokePercent;
                ViewBag.hubPST = hubPercent;
                ViewBag.totalQTY = totalSum;
                ViewBag.totalPST = 100;
            }



            return View("Index", model);
        }



        [HttpPost]
        public IActionResult SelectStore(int weekNo, int storeId,int TabNo)
        {

            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.menu = _menu;
            ViewBag.storeId = storeId;
            ViewBag.store = _context.Store.ToList();
            ViewBag.weekNo = weekNo;
            ViewBag.tabNo = TabNo;
            ViewBag.weekMaster = _context.Week.ToList();

            if (TabNo == 0)
            {
                ViewBag.summary = "active";
            }
            else
            {
                ViewBag.detail = "active";
            }
          
            

            IQueryable<PlanDetail>? modelQuery = _context.PlanDetail.Include(m => m.store).Include(m => m.item);
            if (storeId != 0)
            {
                modelQuery = modelQuery.Where(m => m.store_id == storeId);
            }

            var model = modelQuery.Where(m => m.week_no == weekNo).ToList();

            var plan = _context.PlanDetail.Include(m => m.store).ThenInclude(m => m.store_type).Include(m => m.item).Where(m => m.week_no == weekNo).ToList();

            ViewBag.hubQTY = 0;
            ViewBag.spokeQTY = 0;
            ViewBag.spokePST = 0;
            ViewBag.hubPST = 0;
            ViewBag.totalQTY = 0;
            ViewBag.totalPST = 0;

            if (plan.Count() != 0)
            {
                // เลือกข้อมูลสำหรับ week_no
                var weekThreeData = model.Where(m => m.week_no == weekNo).ToList();

                // หาผลรวมของข้อมูลทั้งหมดใน week_no นั้น
                var totalSum = weekThreeData.Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // หาผลรวมของข้อมูลที่ store_type เป็น "HUB"
                var hubSum = weekThreeData.Where(m => m.store.store_type.store_type_code == "3001")
                    .Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // หาผลรวมของข้อมูลที่ store_type เป็น "SPOKE"
                var spokeSum = weekThreeData.Where(m => m.store.store_type.store_type_code == "3002")
                    .Sum(m => m.plan_mon + m.plan_tues + m.plan_wed + m.plan_thu + m.plan_fri + m.plan_sat + m.plan_sun);

                // คำนวณเป็น percent
                var hubPercent = ((double)hubSum / totalSum * 100).ToString("N2");
                var spokePercent = ((double)spokeSum / totalSum * 100).ToString("N2");
                ViewBag.hubQTY = hubSum;
                ViewBag.spokeQTY = spokeSum;
                ViewBag.spokePST = spokePercent;
                ViewBag.hubPST = hubPercent;
                ViewBag.totalQTY = totalSum;
                ViewBag.totalPST = 100;
            }

            return View("Index",model);
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

        public IActionResult ExportToExcel(int weekNo, int storeId)
        {

            IQueryable<PlanDetail>? modelQuery = _context.PlanDetail
                .Include(m => m.store)
                .ThenInclude(m => m.store_type)
                .Include(m => m.item)
                .Where(m => m.week_no == weekNo);
            if (storeId != 0)
            {
                modelQuery = modelQuery.Where(m => m.store_id == storeId);
            }

            var data = modelQuery.ToList();

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
                worksheet.Cells[1, 1].Value = "store code";
                worksheet.Cells[1, 2].Value = "sku code";
                worksheet.Cells[1, 3].Value = "Plan(Mon.)";
                worksheet.Cells[1, 4].Value = "Plan(Tue.)";
                worksheet.Cells[1, 5].Value = "Plan(Wed.)";
                worksheet.Cells[1, 6].Value = "Plan(Thu.)";
                worksheet.Cells[1, 7].Value = "Plan(Fri.)";
                worksheet.Cells[1, 8].Value = "Plan(Sat.)";
                worksheet.Cells[1, 9].Value = "Plan(Sun.)";
                // Add more columns as needed

                // Data

                for (var i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].store_id;
                    worksheet.Cells[i + 2, 2].Value = data[i].sku_id;
                    worksheet.Cells[i + 2, 3].Value = data[i].plan_mon;
                    worksheet.Cells[i + 2, 4].Value = data[i].plan_tues;
                    worksheet.Cells[i + 2, 5].Value = data[i].plan_wed;
                    worksheet.Cells[i + 2, 6].Value = data[i].plan_thu;
                    worksheet.Cells[i + 2, 7].Value = data[i].plan_fri;
                    worksheet.Cells[i + 2, 8].Value = data[i].plan_sat;
                    worksheet.Cells[i + 2, 9].Value = data[i].plan_sun;
                    // Add more columns as needed
                }

                package.Save(); // Save the Excel package
            }

            stream.Position = 0;

            // Set the content type and file name
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PlanDetail_List.xlsx");
        }
    }
}
