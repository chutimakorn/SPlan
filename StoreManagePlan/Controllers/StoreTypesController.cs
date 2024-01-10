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
using StoreManagePlan.Repository;
using static StoreManagePlan.Controllers.ItemFeaturesController;
using NuGet.Packaging.Core;
using System.Text.Json;
using Elfie.Serialization;
using StoreManagePlan.Repository;
using System.Globalization;
using System.Xml.Linq;

namespace StoreManagePlan.Controllers
{
    public class StoreTypesController : Controller
    {
        IUtility _utility;
        private readonly StoreManagePlanContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public static string _menu = "storeType";
        public StoreTypesController(StoreManagePlanContext context, IUtility utility, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            this._utility = utility;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: StoreTypes
        public async Task<IActionResult> Index()
        {
            var history = _context.ImportLog.Where(m => m.menu == _menu).ToList();
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies.TryGetValue("Role", out string roleValue));
            ViewBag.historyLog = history;
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
            ViewBag.role = HttpContext.Session.GetInt32("Role");
            ResponseStatus jsonData = new ResponseStatus();
            ImportLog log = new ImportLog();
            log.menu = _menu;
            log.create_date = _utility.CreateDate();
            log.old_name = file.FileName;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                if (file != null && file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        using (var package = new ExcelPackage(stream))
                        {
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

                            if (worksheet.Cells[1, 1].Value.ToString() != "store type")
                            {
                                jsonData.status = "unsuccessful";
                                jsonData.message = "invalid file";

                                log.status = jsonData.status;
                                log.message = jsonData.message;
                                _context.Add(log);
                                _context.SaveChanges();

                                return Json(jsonData);
                            }

                            string contentRootPath = _hostingEnvironment.ContentRootPath;
                            DateTime currentDate = DateTime.Now;
                            string dateStringWithMilliseconds = currentDate.ToString("yyyyMMddHHmmssfff");
                            string ext = Path.GetExtension(file.FileName);
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var newName = fileName + "_" + dateStringWithMilliseconds + ext;
                            string yourFilePath = Path.Combine(contentRootPath, "Shared", newName);

                            log.current_name = newName;
                            _utility.SaveExcelFile(package, yourFilePath);


                            var excelDataList = new List<StoreType>();
                            var excelUpdateList = new List<StoreType>();
                            for (int row = 3; row <= rowCount; row++)
                            {
                                var effDate = worksheet.Cells[row, 3];
                                var itemOld =  _context.StoreType.Where(i => i.store_type_code == worksheet.Cells[row, 1].Value.ToString()).SingleOrDefault();

                                if (itemOld != null)
                                {
                                    itemOld.store_type_name = worksheet.Cells[row, 2].Value.ToString();
                                    itemOld.update_date = _utility.CreateDate();
                                    excelUpdateList.Add(itemOld);
                                }
                                else
                                {
                                    excelDataList.Add(new StoreType
                                    {

                                        store_type_code = worksheet.Cells[row, 1].Value.ToString(),
                                        store_type_name = worksheet.Cells[row, 2].Value.ToString(),
                                        create_date = _utility.CreateDate(),
                                        update_date = _utility.CreateDate(),
                                    });
                                }
                            }

                            if (ModelState.IsValid)
                            {
                               
                                    try
                                    {
                                        foreach (var item in excelDataList)
                                        {
                                            _context.StoreType.Add(item);
                                        }

                                        foreach (var item in excelUpdateList)
                                        {
                                            _context.StoreType.Update(item);
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
            var data = _context.StoreType.ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
              
                worksheet.Cells[1, 1].Value = "store type code";
                worksheet.Cells[1, 2].Value = "store type name";
                worksheet.Cells[1, 3].Value = "Create date";
                worksheet.Cells[1, 4].Value = "Update date";
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
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StoreType_List.xlsx");
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
