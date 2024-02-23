using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagePlan.Data;
using StoreManagePlan.Models;

namespace StoreManagePlan.Controllers
{
    public class PlanDetailsController : Controller
    {
        private readonly StoreManagePlanContext _context;

        public PlanDetailsController(StoreManagePlanContext context)
        {
            _context = context;
        }

        // GET: PlanDetails
        public async Task<IActionResult> Index()
        {
            var storeManagePlanContext = _context.PlanDetail.Include(p => p.item).Include(p => p.store);
            return View(await storeManagePlanContext.ToListAsync());
        }

        // GET: PlanDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planDetail = await _context.PlanDetail
                .Include(p => p.item)
                .Include(p => p.store)
                .FirstOrDefaultAsync(m => m.id == id);
            if (planDetail == null)
            {
                return NotFound();
            }

            return View(planDetail);
        }

        // GET: PlanDetails/Create
        public IActionResult Create()
        {
            ViewData["sku_id"] = new SelectList(_context.Item, "id", "id");
            ViewData["store_id"] = new SelectList(_context.Store, "id", "id");
            return View();
        }

        // POST: PlanDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,sku_id,store_id,plan_mon,plan_tues,plan_wed,plan_thu,plan_fri,plan_sat,plan_sun")] PlanDetail planDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(planDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["sku_id"] = new SelectList(_context.Item, "id", "id", planDetail.sku_id);
            ViewData["store_id"] = new SelectList(_context.Store, "id", "id", planDetail.store_id);
            return View(planDetail);
        }

        // GET: PlanDetails/Edit/5
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
            return View(planDetail);
        }

        // POST: PlanDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,sku_id,store_id,plan_mon,plan_tues,plan_wed,plan_thu,plan_fri,plan_sat,plan_sun")] PlanDetail planDetail)
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
            return View(planDetail);
        }

        // GET: PlanDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planDetail = await _context.PlanDetail
                .Include(p => p.item)
                .Include(p => p.store)
                .FirstOrDefaultAsync(m => m.id == id);
            if (planDetail == null)
            {
                return NotFound();
            }

            return View(planDetail);
        }

        // POST: PlanDetails/Delete/5
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
