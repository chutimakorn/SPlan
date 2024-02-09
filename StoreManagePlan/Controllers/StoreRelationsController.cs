using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using StoreManagePlan.Data;
using StoreManagePlan.Models;
using StoreManagePlan.Repository;

namespace StoreManagePlan.Controllers
{
    public class StoreRelationsController : Controller
    {
        private readonly StoreManagePlanContext _context;
        IUtility _utility;
        public StoreRelationsController(StoreManagePlanContext context, IUtility utility)
        {
            _context = context;
            this._utility = utility;
        }

        // GET: StoreRelations
        public async Task<IActionResult> Index(string SpokeID, string hubID,string start,string end)
        {
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies.TryGetValue("Role", out string roleValue));
            ViewBag.menu = "storeRelates";
            ResourceController resource = new ResourceController();

            var storeManagePlanContext = _context.StoreRelation
                .Include(s => s.StoreHub).ThenInclude(m => m.store_type)
                .Include(s => s.StoreSpoke).ThenInclude(m => m.store_type).Where(m => m.store_hub_id != 0);

            var storeHub = _context.Store.Include(m => m.store_type);

            if(hubID != "0" && hubID != null)
            {
                storeManagePlanContext = storeManagePlanContext.Where(m => m.store_hub_id == Convert.ToInt32(hubID));
                ViewBag.hubID = hubID;
            }
            else
            {
                ViewBag.hubID = "0";
            }
            if (SpokeID != "0" && SpokeID != null)
            {
                storeManagePlanContext = storeManagePlanContext.Where(m => m.store_spoke_id == Convert.ToInt32(SpokeID));
                ViewBag.SpokeID = SpokeID;
            }
            else
            {
                ViewBag.SpokeID = "0";
            }

            ViewBag.start = start;
            ViewBag.end = end;

            if (start == null || start == "")
            {
                start = "0";
               
            }
            else
            {
                start = start.Replace("-","");
            }
            if (end == null || end == "")
            {
                end = "9999999";
            }
            else
            {
                end = end.Replace("-", "");
            }
            
            

            storeManagePlanContext = storeManagePlanContext.Where(m => Convert.ToInt32(m.start_date)  >= Convert.ToInt32(start) && Convert.ToInt32(m.end_date) <= Convert.ToInt32(end));

            ViewBag.storeHub = _context.Store.Include(m => m.store_type).Where(m => m.type_id == 6).ToList();
            ViewBag.storeSpoke = _context.Store.Include(m => m.store_type).Where(m => m.type_id == 7).ToList();


            return View(await storeManagePlanContext.ToListAsync());
        }

        // GET: StoreRelations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeRelation = await _context.StoreRelation
                .Include(s => s.StoreHub)
                .Include(s => s.StoreSpoke)
                .FirstOrDefaultAsync(m => m.store_hub_id == id);
            if (storeRelation == null)
            {
                return NotFound();
            }

            return View(storeRelation);
        }

        // GET: StoreRelations/Create
        public IActionResult Create()
        {
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies.TryGetValue("Role", out string roleValue));
            ViewBag.menu = "storeRelates";

            var spokeInHub = _context.StoreRelation
                        .Include(m => m.StoreSpoke)
                        .Select(m => m.store_spoke_id)
                        .ToList();

            // ดึง storeSpoke โดยไม่มี spokeInHub
            var storeSpokeWithoutInHub = _context.Store
                                            .Include(m => m.store_type)
                                            .Where(m => m.type_id == 7 && !spokeInHub.Contains(m.id))
                                            .ToList();

            ViewBag.storeHub = _context.Store.Include(m => m.store_type).Where(m => m.type_id == 6).ToList();
            ViewBag.storeSpoke = storeSpokeWithoutInHub;

            return View();
        }

        // POST: StoreRelations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(string listSpoke ,int hubID, string start, string end)
        {
            if (listSpoke != null && listSpoke != "" && hubID != 0)
            {
                var spokeID = listSpoke.Split(',');
                if (start != null && start != "")
                {
                   start = start.Replace("-", "");
                }
                else
                {
                    start = "";
                }
                if (end != null && end != "")
                {
                    end = end.Replace("-", "");
                }
                else
                {
                    end = "";
                }
                foreach (var n in spokeID)
                {
                    StoreRelation storeRelation = new StoreRelation();
                    storeRelation.store_hub_id = hubID;
                    storeRelation.store_spoke_id = Convert.ToInt16(n);
                    storeRelation.start_date = start;
                    storeRelation.end_date = end;
                    storeRelation.create_date = _utility.CreateDate();
                    storeRelation.update_date = _utility.CreateDate();
                    _context.Add(storeRelation);
                }


              
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies.TryGetValue("Role", out string roleValue));
            ViewBag.menu = "storeRelates";

            var spokeInHub = _context.StoreRelation
                        .Include(m => m.StoreSpoke)
                        .Select(m => m.store_spoke_id)
                        .ToList();

            // ดึง storeSpoke โดยไม่มี spokeInHub
            var storeSpokeWithoutInHub = _context.Store
                                            .Include(m => m.store_type)
                                            .Where(m => m.type_id == 7 && !spokeInHub.Contains(m.id))
                                            .ToList();

            ViewBag.storeHub = _context.Store.Include(m => m.store_type).Where(m => m.type_id == 6).ToList();
            ViewBag.storeSpoke = storeSpokeWithoutInHub;
            return View();
        }

        // GET: StoreRelations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeRelation = await _context.StoreRelation.FindAsync(id);
            if (storeRelation == null)
            {
                return NotFound();
            }
            ViewData["store_hub_id"] = new SelectList(_context.Store, "id", "id", storeRelation.store_hub_id);
            ViewData["store_spoke_id"] = new SelectList(_context.Store, "id", "id", storeRelation.store_spoke_id);
            return View(storeRelation);
        }

        // POST: StoreRelations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("store_hub_id,store_spoke_id,start_date,end_date,create_date,update_date")] StoreRelation storeRelation)
        {
            if (id != storeRelation.store_hub_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storeRelation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreRelationExists(storeRelation.store_hub_id))
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
            ViewData["store_hub_id"] = new SelectList(_context.Store, "id", "id", storeRelation.store_hub_id);
            ViewData["store_spoke_id"] = new SelectList(_context.Store, "id", "id", storeRelation.store_spoke_id);
            return View(storeRelation);
        }

        // GET: StoreRelations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeRelation = await _context.StoreRelation
                .Include(s => s.StoreHub)
                .Include(s => s.StoreSpoke)
                .FirstOrDefaultAsync(m => m.store_hub_id == id);
            if (storeRelation == null)
            {
                return NotFound();
            }

            return View(storeRelation);
        }

        // POST: StoreRelations/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(string selectedID)
        {
            if (selectedID == "")
            {
                return View(await _context.Item.ToListAsync());
            }

            var idList = selectedID.Split(',');

            foreach (var id in idList)
            {

                var itemModel = _context.StoreRelation.Where(m => m.id == Convert.ToInt32(id)).SingleOrDefault();
                if (itemModel != null)
                {
                    _context.StoreRelation.Remove(itemModel);
                }

            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ExportToExcel()
        {
            var data = _context.StoreRelation.Include(m => m.StoreHub).Include(m => m.StoreSpoke).ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
                
                worksheet.Cells[1, 1].Value = "Producer";
                worksheet.Cells[1, 2].Value = "Seller";
                worksheet.Cells[1, 3].Value = "Start";
                worksheet.Cells[1, 4].Value = "End";
                worksheet.Cells[1, 4].Value = "Create date";
                worksheet.Cells[1, 4].Value = "Upate date";

                // Add more columns as needed

                // Data
                for (var i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].StoreHub.store_name;
                    worksheet.Cells[i + 2, 2].Value = data[i].StoreSpoke.store_name;
                    worksheet.Cells[i + 2, 3].Value = data[i].start_date;
                    worksheet.Cells[i + 2, 4].Value = data[i].end_date;
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
        private bool StoreRelationExists(int id)
        {
            return _context.StoreRelation.Any(e => e.store_hub_id == id);
        }
    }
}
