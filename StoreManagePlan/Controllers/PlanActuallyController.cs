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
    public class PlanActuallyController : Controller
    {
        private readonly StoreManagePlanContext _context;

        public PlanActuallyController(StoreManagePlanContext context)
        {
            _context = context;
        }

        // GET: PlanActuallies
        public async Task<IActionResult> Index()
        {
            var storeManagePlanContext = _context.PlanActually.Include(p => p.planDetail).Include(p => p.reason);
            return View(await storeManagePlanContext.ToListAsync());
        }

        // GET: PlanActuallies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planActually = await _context.PlanActually
                .Include(p => p.planDetail)
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
            ViewData["plan_detail_id"] = new SelectList(_context.PlanDetail, "id", "id", planActually.plan_detail_id);
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
            ViewData["plan_detail_id"] = new SelectList(_context.PlanDetail, "id", "id", planActually.plan_detail_id);
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
            ViewData["plan_detail_id"] = new SelectList(_context.PlanDetail, "id", "id", planActually.plan_detail_id);
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
                .Include(p => p.planDetail)
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
