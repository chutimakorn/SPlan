using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using StoreManagePlan.Data;
using StoreManagePlan.Models;

namespace StoreManagePlan.Controllers
{
    public class StoresController : Controller
    {
        private readonly StoreManagePlanContext _context;

        public StoresController(StoreManagePlanContext context)
        {
            _context = context;
        }

        // GET: Stores
        public async Task<IActionResult> Index()
        {
            ViewBag.menu = "store";
            var storeManagePlanContext = _context.Store.Include(s => s.store_type);
            return View(await storeManagePlanContext.ToListAsync());
        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .Include(s => s.store_type)
                .FirstOrDefaultAsync(m => m.id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // GET: Stores/Create
        public IActionResult Create()
        {
            ViewData["type_id"] = new SelectList(_context.StoreType, "id", "id");
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,type_id,store_code,store_name,create_date,update_date")] Store store)
        {
            if (ModelState.IsValid)
            {
                _context.Add(store);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["type_id"] = new SelectList(_context.StoreType, "id", "id", store.type_id);
            return View(store);
        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            ViewData["type_id"] = new SelectList(_context.StoreType, "id", "id", store.type_id);
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,type_id,store_code,store_name,create_date,update_date")] Store store)
        {
            if (id != store.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.id))
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
            ViewData["type_id"] = new SelectList(_context.StoreType, "id", "id", store.type_id);
            return View(store);
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .Include(s => s.store_type)
                .FirstOrDefaultAsync(m => m.id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var store = await _context.Store.FindAsync(id);
            if (store != null)
            {
                _context.Store.Remove(store);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(int id)
        {
            return _context.Store.Any(e => e.id == id);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ResourceController _resource = new ResourceController();

            if (file != null && file.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        var excelDataList = new List<Store>();




                        for (int row = 2; row <= rowCount; row++)
                        {


                            //get item id
                            var stroeTypeID = _context.StoreType.Where(m => m.store_type_code == worksheet.Cells[row, 3].Value.ToString()).Select(m => m.id).SingleOrDefault();


                            excelDataList.Add(new Store
                            {
                            
                                store_code = worksheet.Cells[row, 1].Value.ToString(),
                                store_name = worksheet.Cells[row, 2].Value.ToString(),
                                type_id = stroeTypeID,
                                create_date = _resource.CreateDate(),
                                update_date = _resource.CreateDate(),
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

        public IActionResult ExportToExcel()
        {
            var data = _context.Store.Include(m => m.store_type).ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
                worksheet.Cells[1, 1].Value = "STORE CODE";
                worksheet.Cells[1, 2].Value = "STORE NAME";
                worksheet.Cells[1, 3].Value = "STORE TYPE CODE";
                worksheet.Cells[1, 4].Value = "STORE TYPE NAME";
                worksheet.Cells[1, 5].Value = "CREATE DATE";
                worksheet.Cells[1, 6].Value = "UPDATE DATE";         
                // Add more columns as needed

                // Data
                for (var i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].store_code;
                    worksheet.Cells[i + 2, 2].Value = data[i].store_name;
                    worksheet.Cells[i + 2, 3].Value = data[i].store_type.store_type_code;
                    worksheet.Cells[i + 2, 4].Value = data[i].store_type.store_type_name;
                    worksheet.Cells[i + 2, 5].Value = data[i].create_date;
                    worksheet.Cells[i + 2, 6].Value = data[i].update_date;


                    // Add more columns as needed
                }

                package.Save(); // Save the Excel package
            }

            stream.Position = 0;

            // Set the content type and file name
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Item_List.xlsx");
        }
    }
}
