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
using StoreManagePlan.Repository;
using System.Globalization;
using System.Xml.Linq;
using System.Text.Json;

namespace StoreManagePlan.Controllers
{
    public class ItemFeaturesController : Controller
    {
        IUtility _utility;
        private readonly StoreManagePlanContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public static string _menu = "ItemFeature";

        public ItemFeaturesController(StoreManagePlanContext context, IUtility utility, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            this._utility = utility;
            _hostingEnvironment = hostingEnvironment;

        }

        // GET: ItemFeatures
        public async Task<IActionResult> Index()
        {
            var history = _context.ImportLog.Where(m => m.menu == _menu).ToList();

            ViewBag.historyLog = history;
            ViewBag.menu = "itemFeature";
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies.TryGetValue("Role", out string roleValue));
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

                            if (worksheet.Cells[1, 1].Value.ToString() != "item feature")
                            {
                                jsonData.status = "unsuccessful";
                                jsonData.message = "invalid file";

                                log.status = jsonData.status;
                                log.message = jsonData.message;
                                _context.Add(log);
                                _context.SaveChanges();

                                return Json(jsonData);
                            }

                            var excelDataList = new List<ItemFeature>();
                            var excelUpdateList = new List<ItemFeature>();
                            for (int row = 3; row <= rowCount; row++)
                            {
                                

                                var itemID = _context.Item.Where(m => m.sku_code == worksheet.Cells[row, 2].Value.ToString()).SingleOrDefault();

                                var storeID = _context.Store.Where(m => m.store_code == worksheet.Cells[row, 1].Value.ToString()).SingleOrDefault();

                                if (itemID == null)
                                {
                                    jsonData.status = "unsuccessful";
                                    jsonData.message = "item not found";
                                    log.status = jsonData.status;
                                    log.message = jsonData.message;

                                    _context.Add(log);
                                    _context.SaveChanges();

                                    return Json(jsonData);
                                }

                                if (storeID == null)
                                {
                                    jsonData.status = "unsuccessful";
                                    jsonData.message = "store not found";
                                    log.status = jsonData.status;
                                    log.message = jsonData.message;

                                    _context.Add(log);
                                    _context.SaveChanges();

                                    return Json(jsonData);
                                }

                                var itemOld = _context.ItemFeature.Where(i => i.store_id == storeID.id && i.item_id == itemID.id).SingleOrDefault();

                                if (itemOld != null)
                                {
                                    itemOld.minimum_feature = _utility.GetInt(worksheet.Cells[row, 3]).Value;
                                    itemOld.maximum_feature = _utility.GetInt(worksheet.Cells[row, 4]).Value;
                                    itemOld.default_feature = _utility.GetInt(worksheet.Cells[row, 5]).Value;
                                    excelUpdateList.Add(itemOld);
                                }
                                else
                                {
                                    excelDataList.Add(new ItemFeature
                                    {

                                        store_id = storeID.id,
                                        item_id = itemID.id,
                                        minimum_feature = _utility.GetInt(worksheet.Cells[row, 3]).Value,
                                        maximum_feature = _utility.GetInt(worksheet.Cells[row, 4]).Value,
                                        default_feature = _utility.GetInt(worksheet.Cells[row, 5]).Value,
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
                                        _context.Add(item);
                                    }

                                    foreach (var item in excelUpdateList)
                                    {
                                        _context.Update(item);
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
            var data = _context.ItemFeature.ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
                worksheet.Cells[1, 1].Value = "store code";
                worksheet.Cells[1, 2].Value = "sku code";
                worksheet.Cells[1, 3].Value = "minimum feature";
                worksheet.Cells[1, 4].Value = "maximum feature";
                worksheet.Cells[1, 5].Value = "default feature";
                worksheet.Cells[1, 6].Value = "create date";
                worksheet.Cells[1, 6].Value = "update date";
                // Add more columns as needed

                // Data
                for (var i = 0; i < data.Count; i++)
                {

                    var store = _context.Store.Where(m => m.id == data[i].store_id).SingleOrDefault();
                    var storeType = _context.Item.Where(m => m.id == data[i].item_id).SingleOrDefault();

                    worksheet.Cells[i + 2, 1].Value = store.store_code;
                    worksheet.Cells[i + 2, 2].Value = storeType.sku_code;
                    worksheet.Cells[i + 2, 3].Value = data[i].minimum_feature;
                    worksheet.Cells[i + 2, 4].Value = data[i].maximum_feature;
                    worksheet.Cells[i + 2, 5].Value = data[i].default_feature;
                    worksheet.Cells[i + 2, 6].Value = data[i].create_date;
                    worksheet.Cells[i + 2, 7].Value = data[i].update_date;
                    // Add more columns as needed
                }

                package.Save(); // Save the Excel package
            }

            stream.Position = 0;

            // Set the content type and file name
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ItemFeature_List.xlsx");
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
