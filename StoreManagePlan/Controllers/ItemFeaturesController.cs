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
    public class ItemFeaturesController : Controller
    {
        private readonly StoreManagePlanContext _context;

        public ItemFeaturesController(StoreManagePlanContext context)
        {
            _context = context;
        }

        // GET: ItemFeatures
        public async Task<IActionResult> Index()
        {
            var storeManagePlanContext = _context.ItemFeature.Include(i => i.Item).Include(i => i.Store);
            return View(await storeManagePlanContext.ToListAsync());
        }

        // GET: ItemFeatures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemFeature = await _context.ItemFeature
                .Include(i => i.Item)
                .Include(i => i.Store)
                .FirstOrDefaultAsync(m => m.store_id == id);
            if (itemFeature == null)
            {
                return NotFound();
            }

            return View(itemFeature);
        }

        // GET: ItemFeatures/Create
        public IActionResult Create()
        {
            ViewData["item_id"] = new SelectList(_context.Item, "id", "id");
            ViewData["store_id"] = new SelectList(_context.Store, "id", "id");
            return View();
        }

        // POST: ItemFeatures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("store_id,item_id,minimum_feature,maximum_feature,default_feature,create_date,update_date")] ItemFeature itemFeature)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemFeature);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["item_id"] = new SelectList(_context.Item, "id", "id", itemFeature.item_id);
            ViewData["store_id"] = new SelectList(_context.Store, "id", "id", itemFeature.store_id);
            return View(itemFeature);
        }

        // GET: ItemFeatures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemFeature = await _context.ItemFeature.FindAsync(id);
            if (itemFeature == null)
            {
                return NotFound();
            }
            ViewData["item_id"] = new SelectList(_context.Item, "id", "id", itemFeature.item_id);
            ViewData["store_id"] = new SelectList(_context.Store, "id", "id", itemFeature.store_id);
            return View(itemFeature);
        }

        // POST: ItemFeatures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("store_id,item_id,minimum_feature,maximum_feature,default_feature,create_date,update_date")] ItemFeature itemFeature)
        {
            if (id != itemFeature.store_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemFeature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemFeatureExists(itemFeature.store_id))
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
            ViewData["item_id"] = new SelectList(_context.Item, "id", "id", itemFeature.item_id);
            ViewData["store_id"] = new SelectList(_context.Store, "id", "id", itemFeature.store_id);
            return View(itemFeature);
        }

        // GET: ItemFeatures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemFeature = await _context.ItemFeature
                .Include(i => i.Item)
                .Include(i => i.Store)
                .FirstOrDefaultAsync(m => m.store_id == id);
            if (itemFeature == null)
            {
                return NotFound();
            }

            return View(itemFeature);
        }

        // POST: ItemFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemFeature = await _context.ItemFeature.FindAsync(id);
            if (itemFeature != null)
            {
                _context.ItemFeature.Remove(itemFeature);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemFeatureExists(int id)
        {
            return _context.ItemFeature.Any(e => e.store_id == id);
        }
    }
}
