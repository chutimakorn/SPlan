using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using StoreManagePlan.Data;
using StoreManagePlan.Models;
using static StoreManagePlan.Controllers.ItemFeaturesController;

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
            ViewBag.menu = "storeType";
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

        [HttpPost]
        public async Task<IActionResult> Delete(string selected)
        {
            if (selected == null || selected == "")
            {
                return RedirectToAction(nameof(Index));
            }


            var idSku = selected.Split(',');

            foreach (var code in idSku)
            {

                var itemModel = _context.StoreType.Where(m => m.store_type_code == code).SingleOrDefault();
                if (itemModel != null)
                {
                    _context.StoreType.Remove(itemModel);
                }

            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool StoreTypeExists(int id)
        {
            return _context.StoreType.Any(e => e.id == id);
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

                        var excelDataList = new List<StoreType>();




                        for (int row = 2; row <= rowCount; row++)
                        {

                            excelDataList.Add(new StoreType
                            {

                                store_type_code = worksheet.Cells[row, 1].Value.ToString(),
                                store_type_name = worksheet.Cells[row, 2].Value.ToString(),                              
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
            var data = _context.StoreType.ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
             
                worksheet.Cells[1, 1].Value = "STORE TYPE CODE";
                worksheet.Cells[1, 2].Value = "STORE TYPE NAME";
                worksheet.Cells[1, 3].Value = "CREATE DATE";
                worksheet.Cells[1, 4].Value = "UPDATE DATE";
 
                // Add more columns as needed

                // Data
                for (var i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].store_type_code;
                    worksheet.Cells[i + 2, 2].Value = data[i].store_type_name;               
                    worksheet.Cells[i + 2, 3].Value = data[i].create_date;
                    worksheet.Cells[i + 2, 4].Value = data[i].update_date;


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
