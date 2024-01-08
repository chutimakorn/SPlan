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
using OfficeOpenXml;
using NuGet.Packaging.Core;
using System.Text.Json;
using Elfie.Serialization;
using StoreManagePlan.Repository;
using System.Globalization;
using System.Xml.Linq;
using Newtonsoft.Json;


namespace StoreManagePlan.Controllers
{
    public class BomController : Controller
    {
        IUtility _utility;
        private readonly StoreManagePlanContext _context;

        public BomController(StoreManagePlanContext context, IUtility utility)
        {
            _context = context;
            this._utility = utility;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.menu = "bom";

            var history = _context.ImportLog.Where(m => m.menu == "Bom").ToList();

            ViewBag.historyLog = history;

            return View(await _context.Bom.Include(m => m.Item).ToListAsync());


        }

        public class ItemSelect
        {
            public string ingredient_sku { get; set; }
            public string sku_id { get; set; }
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


                //get item id
                var itemID = _context.Item.Where(m => m.sku_code == item.sku_id).Select(m => m.id).SingleOrDefault();

                var itemFeatureModel = _context.Bom.Include(m => m.Item).Where(m => m.sku_id == itemID && m.ingredient_sku == item.ingredient_sku).SingleOrDefault();
                if (itemFeatureModel != null)
                {
                    _context.Bom.Remove(itemFeatureModel);
                }

            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            CultureInfo culture = new CultureInfo("en-US");
            ResponseStatus jsonData = new ResponseStatus();
            ImportLog log = new ImportLog();
            log.menu = "Bom";
            log.create_date = _utility.CreateDate();
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

                            if (worksheet.Cells[1, 1].Value.ToString() != "bom")
                            {
                                jsonData.status = "unsuccessful";
                                jsonData.message = "invalid file";

                                log.status = jsonData.status;
                                log.message = jsonData.message;

                                _context.Add(log);
                                _context.SaveChanges();

                                return Json(jsonData);
                            }

                            var rowCount = worksheet.Dimension.Rows;

                            var excelDataList = new List<Bom>();
                            var excelUpdateList = new List<Bom>();

                            for (int row = 3; row <= rowCount; row++)
                            {
                                var effDate = worksheet.Cells[row, 3];
                                var itemOld = await _context.Bom.Include(m => m.Item).Where(i => i.Item.sku_code == worksheet.Cells[row, 1].Value.ToString() && i.ingredient_sku == worksheet.Cells[row, 5].Value.ToString()).FirstOrDefaultAsync();

                                //get item id
                                var itemID = _context.Item.Where(m => m.sku_code == worksheet.Cells[row, 1].Value.ToString()).Select(m => m.id).SingleOrDefault();

                                if (itemOld != null)
                                {
                                    itemOld.min_batch_hub = _utility.GetInt(worksheet.Cells[row, 2]);
                                    itemOld.min_batch_non_hub = _utility.GetInt(worksheet.Cells[row, 3]);
                                    itemOld.batch_uom = _utility.GetString(worksheet.Cells[row, 4]);
                                    itemOld.ingredient_name = _utility.GetString(worksheet.Cells[row, 6]);
                                    itemOld.weight_hub = _utility.GetDecimal(worksheet.Cells[row, 7]);
                                    itemOld.weight_uom = _utility.GetString(worksheet.Cells[row, 8]);
                                    itemOld.update_date = _utility.CreateDate();
                                    excelUpdateList.Add(itemOld);
                                }
                                else
                                {
                                    excelDataList.Add(new Bom
                                    {


                                        sku_id = itemID,
                                        min_batch_hub = _utility.GetInt(worksheet.Cells[row, 2]),
                                        create_date = _utility.CreateDate(),
                                        min_batch_non_hub = _utility.GetInt(worksheet.Cells[row, 3]),
                                        batch_uom = _utility.GetString(worksheet.Cells[row, 4]),
                                        ingredient_sku = worksheet.Cells[row, 5].Value.ToString(),
                                        ingredient_name = _utility.GetString(worksheet.Cells[row, 6]),
                                        weight_hub = _utility.GetDecimal(worksheet.Cells[row, 7]),
                                        weight_uom = _utility.GetString(worksheet.Cells[row, 8]),
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
                                jsonData.message = System.Text.Json.JsonSerializer.Serialize(_context.Item.ToList());

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
            //var data = (from b in _context.Bom
            //            join i in _context.Item on b.sku_code equals i.sku_code into bomitems
            //            from i in bomitems.DefaultIfEmpty()  
            //            select new
            //            { 
            //                sku_code = b.sku_code,
            //                sku_name = i.sku_name,
            //                min_batch_hub = b.min_batch_hub,
            //                min_batch_non_hub = b.min_batch_non_hub,
            //                batch_uom = b.batch_uom,
            //                ingredient_sku = b.ingredient_sku,
            //                ingredient_name = b.ingredient_name,
            //                weight_hub = b.weight_hub,
            //                weight_uom = b.weight_uom,
            //                create_date = b.create_date,
            //                update_date = b.update_date,
            //            }
            //     )
            //    .ToList();

            var data = _context.Bom.Include(m => m.Item).ToList();
                
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
                worksheet.Cells[1, 1].Value = "sku code";
                worksheet.Cells[1, 2].Value = "sku name";
                worksheet.Cells[1, 3].Value = "min batch hub";
                worksheet.Cells[1, 4].Value = "min batch non hub";
                worksheet.Cells[1, 5].Value = "batch uom";
                worksheet.Cells[1, 6].Value = "ingredient sku";
                worksheet.Cells[1, 7].Value = "ingredient name";
                worksheet.Cells[1, 8].Value = "weight hub";
                worksheet.Cells[1, 9].Value = "weight uom";
                worksheet.Cells[1, 10].Value = "create date";
                worksheet.Cells[1, 11].Value = "update date";
                // Add more columns as needed

                // Data
                for (var i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].Item.sku_code;
                    worksheet.Cells[i + 2, 2].Value = data[i].Item.sku_name;
                    worksheet.Cells[i + 2, 3].Value = data[i].min_batch_hub;
                    worksheet.Cells[i + 2, 4].Value = data[i].min_batch_non_hub;
                    worksheet.Cells[i + 2, 5].Value = data[i].batch_uom;
                    worksheet.Cells[i + 2, 6].Value = data[i].ingredient_sku;
                    worksheet.Cells[i + 2, 7].Value = data[i].ingredient_name;
                    worksheet.Cells[i + 2, 8].Value = data[i].weight_hub;
                    worksheet.Cells[i + 2, 9].Value = data[i].weight_uom;
                    worksheet.Cells[i + 2, 10].Value = data[i].create_date;
                    worksheet.Cells[i + 2, 11].Value = data[i].update_date;
                    // Add more columns as needed
                }

                package.Save(); // Save the Excel package
            }

            stream.Position = 0;

            // Set the content type and file name
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Bom_List.xlsx");
        }
    }
}
