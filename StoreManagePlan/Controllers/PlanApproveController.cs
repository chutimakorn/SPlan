using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class PlanApproveController : Controller
    {
        private readonly StoreManagePlanContext _context;
        IUtility _utility;
      
        private readonly IWebHostEnvironment _hostingEnvironment;
        public static string _menu = "pap";
        private readonly IConfiguration _configuration;

        public PlanApproveController(StoreManagePlanContext context, IUtility utility, IConfiguration configuration)
        {
            _context = context;
            this._utility = utility;
            _configuration = configuration;
        }

        // GET: ProductPlanReviewController
        public ActionResult Index(int weekNo, int storeId, int TabNo)
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
                weekNo = Convert.ToInt16(_configuration.GetSection("DefaultWeek").Value);
                ViewBag.weekNo = Convert.ToInt16(_configuration.GetSection("DefaultWeek").Value);
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
        public async Task<IActionResult> Approve(int weekNo, int storeId)
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

            weekMaster.status = 2;

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
        public async Task<IActionResult> Reject(int weekNo, int storeId)
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

            weekMaster.status = 3;

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
        public IActionResult SelectStore(int weekNo, int storeId, int TabNo)
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
            return View("Index", model);
        }

        // GET: PlanApprove/Create
        public IActionResult Create()
        {
            ViewData["sku_id"] = new SelectList(_context.Item, "id", "id");
            ViewData["store_id"] = new SelectList(_context.Store, "id", "id");
            ViewData["week_no"] = new SelectList(_context.Week, "week_no", "week_no");
            return View();
        }

        // POST: PlanApprove/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,sku_id,store_id,plan_mon,plan_tues,plan_wed,plan_thu,plan_fri,plan_sat,plan_sun,week_no")] PlanDetail planDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(planDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["sku_id"] = new SelectList(_context.Item, "id", "id", planDetail.sku_id);
            ViewData["store_id"] = new SelectList(_context.Store, "id", "id", planDetail.store_id);
            ViewData["week_no"] = new SelectList(_context.Week, "week_no", "week_no", planDetail.week_no);
            return View(planDetail);
        }

        // GET: PlanApprove/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planDetail = await _context.PlanDetail.FindAsync(id);
            if (planDetail == null)
            {
                return NotFound();
            }
            ViewData["sku_id"] = new SelectList(_context.Item, "id", "id", planDetail.sku_id);
            ViewData["store_id"] = new SelectList(_context.Store, "id", "id", planDetail.store_id);
            ViewData["week_no"] = new SelectList(_context.Week, "week_no", "week_no", planDetail.week_no);
            return View(planDetail);
        }

        // POST: PlanApprove/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,sku_id,store_id,plan_mon,plan_tues,plan_wed,plan_thu,plan_fri,plan_sat,plan_sun,week_no")] PlanDetail planDetail)
        {
            if (id != planDetail.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(planDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanDetailExists(planDetail.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["sku_id"] = new SelectList(_context.Item, "id", "id", planDetail.sku_id);
            ViewData["store_id"] = new SelectList(_context.Store, "id", "id", planDetail.store_id);
            ViewData["week_no"] = new SelectList(_context.Week, "week_no", "week_no", planDetail.week_no);
            return View(planDetail);
        }

        // GET: PlanApprove/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planDetail = await _context.PlanDetail
                .Include(p => p.item)
                .Include(p => p.store)
                .Include(p => p.week)
                .FirstOrDefaultAsync(m => m.id == id);
            if (planDetail == null)
            {
                return NotFound();
            }

            return View(planDetail);
        }

        // POST: PlanApprove/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var planDetail = await _context.PlanDetail.FindAsync(id);
            if (planDetail != null)
            {
                _context.PlanDetail.Remove(planDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanDetailExists(int id)
        {
            return _context.PlanDetail.Any(e => e.id == id);
        }
    }
}
