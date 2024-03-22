using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
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
    public class PlanActuallyController : Controller
    {
        private readonly StoreManagePlanContext _context;
        IUtility _utility;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public static string _menu = "pac";
        private readonly IConfiguration _configuration;


        public PlanActuallyController(StoreManagePlanContext context, IUtility utility, IConfiguration configuration)
        {
            _context = context;
            this._utility = utility;
            _configuration = configuration;
        }

        public class Plan
        {
            public int Id { get; set; }
            public int plandetail_Id { get; set; }
            public string sku_code { get; set; }
            public string sku_name { get; set; }
            public int value { get; set; }
        
        }

        // GET: PlanActuallies
        public async Task<IActionResult> Index(int storeID,int week,int day,string cycle,int value)
        {
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.storeID = storeID;
            ViewBag.week = week;
            ViewBag.day = day;
            ViewBag.cycle = cycle;
            ViewBag.value = value;
            ViewBag.store = _context.Store.Include(m => m.store_type).Where(n => n.store_type.store_type_name == "Hub").ToList();
            ViewBag.weekMaster = _context.Week.ToList();

            var planDetailApprove = _context.PlanDetail.Include(m => m.item)
                                                       .Include(m => m.store).ThenInclude(m => m.store_type)
                                                       .Include(m => m.week)
                                                       .Where(m => m.approve == true);

           

            List<Plan> plan = new List<Plan>();
            // Sum plandetail by day with sku_name
            IQueryable<Plan> summedPlanDetail;
            if(day != 0 && storeID != 0 && week != 0 && (cycle != null && cycle != ""))
            {
                planDetailApprove = planDetailApprove.Where(m => m.store_id == storeID && m.week_no == week && m.store.store_type.store_type_name == cycle);

                switch (day)
                {
                    case 1:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,                                                             
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_mon)
                                                              });
                        break;
                    case 2:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_tues)
                                                              });
                        break;
                    case 3:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_wed)
                                                              });
                        break;
                    case 4:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_thu)
                                                              });
                        break;
                    case 5:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_fri)
                                                              });
                        break;
                    case 6:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_sat)
                                                              });
                        break;
                    case 7:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_sun)
                                                              });
                        break;
                    default:
                        // Handle invalid day value
                        return BadRequest("Invalid day value.");
                }

                plan = summedPlanDetail.ToList();
            }
         


            
            return View(plan);
        }

        public ActionResult SkuDetail(int id)
        {
            // ตัวอย่างข้อมูลที่จะส่งกลับ
            var skuDetails = new List<object>
            {
                new { store = "Store A", quantity = 10 },
                new { store = "Store B", quantity = 15 },
                new { store = "Store C", quantity = 20 }
            };

            return Json(skuDetails);
        }

        // GET: PlanActuallies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planActually = await _context.PlanActually
               
                .Include(p => p.reason)
                .FirstOrDefaultAsync(m => m.id == id);
            if (planActually == null)
            {
                return NotFound();
            }

            return View(planActually);
        }

        // GET: PlanActuallies/Create
        public IActionResult Create()
        {
            ViewData["plan_detail_id"] = new SelectList(_context.PlanDetail, "id", "id");
            ViewData["reason_id"] = new SelectList(_context.Set<Reason>(), "id", "id");
            return View();
        }

        // POST: PlanActuallies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,plan_detail_id,day_of_week,plan_actually,reason_id,approve")] PlanActually planActually)
        {
            if (ModelState.IsValid)
            {
                _context.Add(planActually);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        
            ViewData["reason_id"] = new SelectList(_context.Set<Reason>(), "id", "id", planActually.reason_id);
            return View(planActually);
        }

        // GET: PlanActuallies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planActually = await _context.PlanActually.FindAsync(id);
            if (planActually == null)
            {
                return NotFound();
            }
            ViewData["reason_id"] = new SelectList(_context.Set<Reason>(), "id", "id", planActually.reason_id);
            return View(planActually);
        }

        // POST: PlanActuallies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,plan_detail_id,day_of_week,plan_actually,reason_id,approve")] PlanActually planActually)
        {
            if (id != planActually.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(planActually);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanActuallyExists(planActually.id))
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
      
            ViewData["reason_id"] = new SelectList(_context.Set<Reason>(), "id", "id", planActually.reason_id);
            return View(planActually);
        }

        // GET: PlanActuallies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planActually = await _context.PlanActually
            
                .Include(p => p.reason)
                .FirstOrDefaultAsync(m => m.id == id);
            if (planActually == null)
            {
                return NotFound();
            }

            return View(planActually);
        }

        // POST: PlanActuallies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var planActually = await _context.PlanActually.FindAsync(id);
            if (planActually != null)
            {
                _context.PlanActually.Remove(planActually);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanActuallyExists(int id)
        {
            return _context.PlanActually.Any(e => e.id == id);
        }
    }
}
