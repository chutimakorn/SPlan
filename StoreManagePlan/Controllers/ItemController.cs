using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagePlan.Context;
using StoreManagePlan.Data;
using StoreManagePlan.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace StoreManagePlan.Controllers
{
    public class ItemController : Controller
    {
        ItemContext db = new ItemContext();
        private readonly StoreManagePlanContext _context;

        public ItemController(StoreManagePlanContext context)
        {
            _context = context;
        }

        // GET: ItemModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Item.ToListAsync());
        }

        // GET: ItemModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemModel = await _context.Item
                .FirstOrDefaultAsync(m => m.id == id);
            if (itemModel == null)
            {
                return NotFound();
            }

            return View(itemModel);
        }

        // GET: ItemModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ItemModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,SKU_CODE,SKU_NAME")] Item itemModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(itemModel);
        }

        // GET: ItemModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemModel = await _context.Item.FindAsync(id);
            if (itemModel == null)
            {
                return NotFound();
            }
            return View(itemModel);
        }

        // POST: ItemModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,SKU_CODE,SKU_NAME")] Item itemModel)
        {
            if (id != itemModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemModelExists(itemModel.id))
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
            return View(itemModel);
        }

        // GET: ItemModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemModel = await _context.Item
                .FirstOrDefaultAsync(m => m.id == id);
            if (itemModel == null)
            {
                return NotFound();
            }

            return View(itemModel);
        }

        // POST: ItemModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemModel = await _context.Item.FindAsync(id);
            if (itemModel != null)
            {
                _context.Item.Remove(itemModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemModelExists(int id)
        {
            return _context.Item.Any(e => e.id == id);
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file != null && file.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        var excelDataList = new List<Item>();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            excelDataList.Add(new Item
                            {
                                id = int.Parse(worksheet.Cells[row, 1].Value.ToString()),
                                sku_code = worksheet.Cells[row, 2].Value.ToString(),
                                sku_name = worksheet.Cells[row, 3].Value.ToString(),
                                create_date = worksheet.Cells[row, 4].Value.ToString(),
                                effective_date = worksheet.Cells[row, 5].Value.ToString(),
                                update_date = worksheet.Cells[row, 6].Value.ToString(),
                                // Add other properties as needed
                            });
                        }

                        // Process the imported data (you can save it to a database, etc.)
                        // Example: SaveToDatabase(excelDataList);
                        if (ModelState.IsValid)
                        {
                            foreach (var item in excelDataList)
                            {
                                _context.Add(item);
                            }
                            await _context.SaveChangesAsync();
                            return RedirectToAction("index");
                        }
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
