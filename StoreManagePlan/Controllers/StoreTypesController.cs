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
    public class StoreTypesController : Controller
    {
        private readonly StoreManagePlanContext _context;

        public StoreTypesController(StoreManagePlanContext context)
        {
            _context = context;
        }

        // GET: StoreTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.StoreType.ToListAsync());
        }

        // GET: StoreTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeType = await _context.StoreType
                .FirstOrDefaultAsync(m => m.id == id);
            if (storeType == null)
            {
                return NotFound();
            }

            return View(storeType);
        }

        // GET: StoreTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StoreTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,store_type_code,store_type_name,create_date,update_date")] StoreType storeType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storeType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(storeType);
        }

        // GET: StoreTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeType = await _context.StoreType.FindAsync(id);
            if (storeType == null)
            {
                return NotFound();
            }
            return View(storeType);
        }

        // POST: StoreTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,store_type_code,store_type_name,create_date,update_date")] StoreType storeType)
        {
            if (id != storeType.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storeType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreTypeExists(storeType.id))
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
            return View(storeType);
        }

        // GET: StoreTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeType = await _context.StoreType
                .FirstOrDefaultAsync(m => m.id == id);
            if (storeType == null)
            {
                return NotFound();
            }

            return View(storeType);
        }

        // POST: StoreTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storeType = await _context.StoreType.FindAsync(id);
            if (storeType != null)
            {
                _context.StoreType.Remove(storeType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreTypeExists(int id)
        {
            return _context.StoreType.Any(e => e.id == id);
        }
    }
}
