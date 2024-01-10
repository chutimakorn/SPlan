using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagePlan.Data;
using StoreManagePlan.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using NuGet.Packaging.Core;
using System.Text.Json;
using Elfie.Serialization;
using StoreManagePlan.Repository;
using System.Globalization;
using System.Xml.Linq;


namespace StoreManagePlan.Controllers
{
    public class ItemController : Controller
    {
        IUtility _utility;
        private readonly StoreManagePlanContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public static string _menu = "Item";

        public ItemController(StoreManagePlanContext context, IUtility utility, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            this._utility = utility;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: ItemModels
        public async Task<IActionResult> Index()
        {
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies.TryGetValue("Role", out string roleValue));
            var history = _context.ImportLog.Where(m => m.menu == _menu).ToList();

            ViewBag.historyLog = history;
            ViewBag.menu = "item";
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
        [HttpPost]
        public async Task<IActionResult> Delete(string selectedSkus)
        {
            if(selectedSkus == "")
            {
                return View(await _context.Item.ToListAsync());
            }

            var idSku = selectedSkus.Split(',');
           
            foreach (var skus in idSku)
            {

                var itemModel = _context.Item.Where(m => m.sku_code == skus).SingleOrDefault();
                if (itemModel != null)
                {
                    _context.Item.Remove(itemModel);
                }

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
            ResponseStatus jsonData = new ResponseStatus();
            ImportLog log = new ImportLog();
            log.menu = _menu;
            log.create_date = _utility.CreateDate();
            log.old_name = file.FileName;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try {
                if (file != null && file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            string contentRootPath = _hostingEnvironment.ContentRootPath;
                            DateTime currentDate = DateTime.Now;
                            string dateStringWithMilliseconds = currentDate.ToString("yyyyMMddHHmmssfff");
                            string ext = Path.GetExtension(file.FileName);
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var newName = fileName + "_" + dateStringWithMilliseconds + ext;
                            string yourFilePath = Path.Combine(contentRootPath, "Shared", newName);

                            log.current_name = newName;
                            _utility.SaveExcelFile(package, yourFilePath);

                            var worksheet = package.Workbook.Worksheets[0];
                            var rowCount = worksheet.Dimension.Rows;

                            if (rowCount < 3)
                            {
                                jsonData.status = "unsuccessful";
                                jsonData.message = "data is 0 row";

                                log.status = jsonData.status;
                                log.message = jsonData.message;
                                _context.Add(log);
                                _context.SaveChanges();

                                return Json(jsonData);
                            }

                            if (worksheet.Cells[1, 1].Value.ToString() != "item") {
                                jsonData.status = "unsuccessful";
                                jsonData.message = "invalid file";

                                log.status = jsonData.status;
                                log.message = jsonData.message;
                                _context.Add(log);
                                _context.SaveChanges();

                                return Json(jsonData);
                            }

                            var excelDataList = new List<Item>();
                            var excelUpdateList = new List<Item>();
                            for (int row = 3; row <= rowCount; row++)
                            {
                                var effDate = worksheet.Cells[row, 3];
                                var itemOld = await _context.Item.Where(i => i.sku_code == worksheet.Cells[row, 1].Value.ToString()).FirstOrDefaultAsync();

                                if (itemOld != null)
                                {
                                    itemOld.sku_name = worksheet.Cells[row, 2].Value.ToString();
                                    itemOld.update_date = _utility.CreateDate();
                                    itemOld.effective_date = effDate.Value == null ? null : worksheet.Cells[row, 3].Value.ToString();
                                    excelUpdateList.Add(itemOld);
                                }
                                else
                                {
                                    excelDataList.Add(new Item
                                    {

                                        sku_code = worksheet.Cells[row, 1].Value.ToString(),
                                        sku_name = worksheet.Cells[row, 2].Value.ToString(),
                                        create_date = _utility.CreateDate(),
                                        effective_date = effDate.Value == null ? null : worksheet.Cells[row, 3].Value.ToString(),
                                    });

                                }


                                
                            }

                            if (ModelState.IsValid)
                            {
                                foreach (var item in excelDataList)
                                {
                                    _context.Add(item);
                                }
                                foreach (var item in excelUpdateList)
                                {
                                    _context.Update(item);
                                }
                                await _context.SaveChangesAsync();

                                jsonData.status = "success";
                                //jsonData.message = JsonSerializer.Serialize(_context.Item.ToList());

                            }
                        }
                    }
                }
                else
                {
                    jsonData.status = "unsuccessful";
                    jsonData.message = "no data";
                }
            }
            catch (Exception ex) {
                jsonData.status = "unsuccessful";
                jsonData.message = ex.Message;
            }

            log.status = jsonData.status;
            log.message = jsonData.message;


            _context.Add(log);

            _context.SaveChanges();

            return Json(jsonData);

        }

        public IActionResult ExportToExcel()
        {
            var data = _context.Item.ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "sku code";
                worksheet.Cells[1, 3].Value = "sku name";
                worksheet.Cells[1, 4].Value = "Create date";
                worksheet.Cells[1, 5].Value = "Update date";
                worksheet.Cells[1, 6].Value = "Effective date";
                // Add more columns as needed

                // Data
                for (var i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].id;
                    worksheet.Cells[i + 2, 2].Value = data[i].sku_code;
                    worksheet.Cells[i + 2, 3].Value = data[i].sku_name;
                    worksheet.Cells[i + 2, 4].Value = data[i].create_date;
                    worksheet.Cells[i + 2, 5].Value = data[i].update_date;
                    worksheet.Cells[i + 2, 6].Value = data[i].effective_date;
                    // Add more columns as needed
                }

                package.Save(); // Save the Excel package
            }

            stream.Position = 0;

            // Set the content type and file name
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Item_List.xlsx");
        }

        public IActionResult DownloadImportFile(int id)
        {

            var log = _context.ImportLog.Where(i => i.id == id).FirstOrDefault();

            if (log == null || log.current_name == null)
            {
                return NotFound();
            }

            string contentRootPath = _hostingEnvironment.ContentRootPath;
            string yourFilePath = Path.Combine(contentRootPath, "Shared", log.current_name);

            if (!System.IO.File.Exists(yourFilePath))
            {
                return NotFound();
            }

            byte[] fileContents = System.IO.File.ReadAllBytes(yourFilePath);

            return File(fileContents, "application/octet-stream", log.old_name);
        }
    }
}
