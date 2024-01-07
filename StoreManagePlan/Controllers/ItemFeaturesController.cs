using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;
using OfficeOpenXml;
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
            ViewBag.menu = "itemFeature";
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

        // POST: ItemModels/Delete/5

        public class ItemSelect
        {
            public string store_code { get; set; }
            public string sku_code { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string selected)
        {
            if (selected == null || selected == "")
            {
                return RedirectToAction(nameof(Index));
            }

            List<ItemSelect> itemList = JsonConvert.DeserializeObject<List<ItemSelect>>(selected);



         
            foreach (var item in itemList)
            {
                //get store id
                var storeID = _context.Store.Where(m => m.store_code == item.store_code).Select(m => m.id).SingleOrDefault();

                //get item id
                var itemID = _context.Item.Where(m => m.sku_code == item.sku_code).Select(m => m.id).SingleOrDefault();


                var itemFeatureModel = _context.ItemFeature.Where(m => m.store_id == Convert.ToInt32(storeID) && m.item_id == Convert.ToInt32(itemID)).SingleOrDefault();
                if (itemFeatureModel != null)
                {
                    _context.ItemFeature.Remove(itemFeatureModel);
                }

            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ItemFeatureExists(int id)
        {
            return _context.ItemFeature.Any(e => e.store_id == id);
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

                        var excelDataList = new List<ItemFeature>();




                        for (int row = 2; row <= rowCount; row++)
                        {

                            //get store id
                            var storeID = _context.Store.Where(m => m.store_code == worksheet.Cells[row, 1].Value.ToString()).Select(m => m.id).SingleOrDefault();

                            //get item id
                            var itemID = _context.Item.Where(m => m.sku_code == worksheet.Cells[row, 2].Value.ToString()).Select(m => m.id).SingleOrDefault();

                            excelDataList.Add(new ItemFeature
                            {
                                store_id = storeID,
                                item_id = itemID,
                                minimum_feature = int.Parse(worksheet.Cells[row, 3].Value.ToString()),
                                maximum_feature = int.Parse(worksheet.Cells[row, 4].Value.ToString()),
                                default_feature = int.Parse(worksheet.Cells[row, 5].Value.ToString()),
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
            var data = _context.ItemFeature.Include(m => m.Store).ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
                worksheet.Cells[1, 1].Value = "STORE CODE";
                worksheet.Cells[1, 2].Value = "STORE NAME";
                worksheet.Cells[1, 3].Value = "SKU CODE";
                worksheet.Cells[1, 4].Value = "ITEM NAME";
                worksheet.Cells[1, 5].Value = "MINIMUM FEATURE";
                worksheet.Cells[1, 6].Value = "MAXIMUM FEATURE";
                worksheet.Cells[1, 7].Value = "DEFAULT VALUE";
                worksheet.Cells[1, 8].Value = "CEATE DATE";
                worksheet.Cells[1, 9].Value = "UPDATE DATE";
                // Add more columns as needed

                // Data
                for (var i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].Store.store_code;
                    worksheet.Cells[i + 2, 2].Value = data[i].Store.store_name;
                    worksheet.Cells[i + 2, 3].Value = data[i].Item.sku_code;
                    worksheet.Cells[i + 2, 4].Value = data[i].Item.sku_name;
                    worksheet.Cells[i + 2, 5].Value = data[i].minimum_feature;
                    worksheet.Cells[i + 2, 6].Value = data[i].maximum_feature;
                    worksheet.Cells[i + 2, 7].Value = data[i].default_feature;
                    worksheet.Cells[i + 2, 8].Value = data[i].create_date;
                    worksheet.Cells[i + 2, 9].Value = data[i].update_date;
                  
               
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
