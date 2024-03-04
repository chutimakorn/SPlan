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
using System.Collections.Generic;
using System.IO;
using NuGet.Packaging.Core;
using System.Text.Json;
using Elfie.Serialization;
using StoreManagePlan.Repository;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.IdentityModel.Tokens;

namespace StoreManagePlan.Controllers
{
    public class StoresController : Controller
    {
        IUtility _utility;
        private readonly StoreManagePlanContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public static string _menu = "store";
        public StoresController(StoreManagePlanContext context, IWebHostEnvironment hostingEnvironment, IUtility utility)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _utility = utility;
        }

        // GET: Stores
        public async Task<IActionResult> Index()
        {
            var history = _context.ImportLog.Where(m => m.menu == _menu).ToList();

            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.historyLog = history;
            ViewBag.menu = _menu;
       
            return View(await _context.Store.Include(m => m.store_type).ToListAsync());
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
            ResponseStatus jsonData = new ResponseStatus();
            ImportLog log = new ImportLog();
            log.menu = _menu;
            log.create_date = _utility.CreateDate();
            log.old_name = file.FileName;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                ViewBag.role = HttpContext.Session.GetInt32("Role");
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

                            if (worksheet.Cells[1, 1].Value.ToString() != "store")
                            {
                                jsonData.status = "unsuccessful";
                                jsonData.message = "invalid file";

                                log.status = jsonData.status;
                                log.message = jsonData.message;
                                _context.Add(log);
                                _context.SaveChanges();

                                return Json(jsonData);
                            }

                            var excelDataList = new List<Store>();
                            var excelUpdateList = new List<Store>();
                            for (int row = 3; row <= rowCount; row++)
                            {
                                
                                var itemOld =  _context.Store.Where(i => i.store_code == worksheet.Cells[row, 2].Value.ToString()).SingleOrDefault();

                                var storeType = _context.StoreType.Where(i => i.store_type_code == worksheet.Cells[row, 1].Value.ToString()).SingleOrDefault();


                                var storeName = worksheet.Cells[row, 3].Value.ToString();
                                var startDate = worksheet.Cells[row, 4].Value.ToString();
                                var endDate = worksheet.Cells[row, 5].Value.ToString();


                                if (storeType == null)
                                {
                                    jsonData.status = "unsuccessful";
                                    jsonData.message = "sku id is null";

                                    log.status = jsonData.status;
                                    log.message = jsonData.message;

                                    _context.Add(log);
                                    _context.SaveChanges();

                                    return Json(jsonData);
                                }

                                if (itemOld != null)
                                {
                                    itemOld.type_id = storeType.id;
                                    itemOld.store_code = itemOld.store_code;
                                    itemOld.store_name = storeName;
                                    itemOld.update_date = _utility.CreateDate();
                                    itemOld.start_date = startDate;
                                    itemOld.end_date = endDate;
                                    excelUpdateList.Add(itemOld);
                                }
                                else
                                {
                                    excelDataList.Add(new Store
                                    {

                                        type_id = storeType.id,
                                        store_name = storeName,
                                        store_code = worksheet.Cells[row, 2].Value.ToString(),
                                        start_date = startDate,
                                        create_date = _utility.CreateDate(), 
                                        update_date = _utility.CreateDate(),
                                    end_date = endDate,
                                    });

                                }



                            }

                        

                            if (ModelState.IsValid)
                            {
                                
                                    try
                                    {
                                        foreach (var item in excelDataList)
                                        {
                                            _context.Store.Add(item);
                                        }

                                        foreach (var item in excelUpdateList)
                                        {
                                            _context.Store.Update(item);
                                        }

                                      
                                      

                                        jsonData.status = "success";
                                    }
                                    catch (Exception ex)
                                    {
                                       
                                        jsonData.status = "unsuccessful";
                                        jsonData.message = ex.Message;
                                        // คุณสามารถเพิ่มข้อมูลเพิ่มเติมใน message ได้ตามความเหมาะสม
                                    }
                               
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
            catch (Exception ex)
            {
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
            var data = _context.Store.ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
               
                worksheet.Cells[1, 1].Value = "type code";
                worksheet.Cells[1, 2].Value = "store code";
                worksheet.Cells[1, 3].Value = "store name";
                worksheet.Cells[1, 4].Value = "Start date";
                worksheet.Cells[1, 5].Value = "End date";
                // Add more columns as needed

                // Data
                for (var i = 0; i < data.Count; i++)
                {
                    var storeType = _context.StoreType.Where(m => m.id == data[i].type_id).SingleOrDefault();

                    worksheet.Cells[i + 2, 1].Value = storeType.store_type_code;
                    worksheet.Cells[i + 2, 2].Value = data[i].store_code;
                    worksheet.Cells[i + 2, 3].Value = data[i].store_name;
                    worksheet.Cells[i + 2, 4].Value = data[i].start_date;
                    worksheet.Cells[i + 2, 5].Value = data[i].end_date;
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
