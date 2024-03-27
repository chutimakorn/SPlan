using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagePlan.Data;
using StoreManagePlan.Models;
using StoreManagePlan.Repository;
using Newtonsoft.Json;
using static System.Formats.Asn1.AsnWriter;


namespace StoreManagePlan.Controllers
{
    public class PlanActuallyController : Controller
    {
        private readonly StoreManagePlanContext _context;
        IUtility _utility;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public static string _menu = "pac";
        private readonly IConfiguration _configuration;


        public PlanActuallyController(StoreManagePlanContext context, IUtility utility, IConfiguration configuration)
        {
            _context = context;
            this._utility = utility;
            _configuration = configuration;
        }

        public class Plan
        {
            public int Id { get; set; }
            public int plandetail_Id { get; set; }
            public string sku_code { get; set; }
            public string sku_name { get; set; }
            public int reason { get; set; }
            public int value { get; set; }
            public int Actualvalue { get; set; }
            public int approve { get; set; }

        }

        // GET: PlanActuallies
        public async Task<IActionResult> Index(int storeID, int week, int day, string cycle)
        {
            ViewBag.role = Convert.ToInt32(HttpContext.Request.Cookies["Role"]);
            ViewBag.storeID = storeID;
            ViewBag.week = week;
            ViewBag.day = day;
            ViewBag.cycle = cycle;
           
            ViewBag.store = _context.Store.Include(m => m.store_type).Where(n => n.store_type.store_type_name == "Hub").ToList();
            ViewBag.weekMaster = _context.Week.ToList();
            ViewBag.reasonhight = _context.Reason.Where(m => m.menu == "pac" && m.type == 1).ToList();
            ViewBag.resonlow = _context.Reason.Where(m => m.menu == "pac" && m.type == 0).ToList();
            ViewBag.reson = _context.Reason.Where(m => m.menu == "pac").ToList();

            var planDetailApprove = _context.PlanDetail.Include(m => m.item)
                                                       .Include(m => m.store).ThenInclude(m => m.store_type)
                                                       .Include(m => m.week)
                                                       .Where(m => m.approve == true);

            var checkAllSubmit = 0;


            List<Plan> plan = new List<Plan>();
            // Sum plandetail by day with sku_name
            IQueryable<Plan> summedPlanDetail;
            if (day != 0 && storeID != 0 && week != 0 && (cycle != null && cycle != ""))
            {
                var actuallyPlan = _context.PlanActually.Where(m => m.week_no == week);
                var reasonPlan = _context.PlanActually.Where(m => m.week_no == week);
                if (cycle == "Hub")
                {

                    planDetailApprove = planDetailApprove.Where(m => m.store_id == storeID && m.week_no == week);
                    actuallyPlan = actuallyPlan.Where(m => m.store_id == storeID);
                    reasonPlan = reasonPlan.Where(m => m.store_id == storeID);

                }
                else
                {
                    var spoke = _context.StoreRelation.Where(m => m.store_hub_id == storeID).Select(m => m.store_spoke_id).ToList();
                    planDetailApprove = planDetailApprove.Where(m => spoke.Contains(m.store_id) && m.week_no == week && m.store.store_type.store_type_name == cycle);
                    actuallyPlan = actuallyPlan.Where(m => spoke.Contains(m.store_id));
                    reasonPlan = reasonPlan.Where(m => spoke.Contains(m.store_id));
                }

              

                switch (day)
                {
                    case 1:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_mon)
                                                              });
                        actuallyPlan = actuallyPlan.Where(m => m.day_of_week == day).GroupBy(m => new { m.sku_id })
                                                          .Select(g => new PlanActually
                                                          {
                                                              sku_id = g.Key.sku_id,
                                                              plan_actually = g.Sum(m => m.plan_actually)
                                                          });
                       
                        break;
                    case 2:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_tues)
                                                              });
                        actuallyPlan = actuallyPlan.Where(m => m.day_of_week == day).GroupBy(m => new { m.sku_id })
                                                          .Select(g => new PlanActually
                                                          {
                                                              sku_id = g.Key.sku_id,
                                                              plan_actually = g.Sum(m => m.plan_actually)
                                                          });
                       
                        break;
                    case 3:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_wed)
                                                              });
                        actuallyPlan = actuallyPlan.Where(m => m.day_of_week == day).GroupBy(m => new { m.sku_id })
                                                          .Select(g => new PlanActually
                                                          {
                                                              sku_id = g.Key.sku_id,
                                                              plan_actually = g.Sum(m => m.plan_actually)
                                                          });

                        break;
                    case 4:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_thu)
                                                              });
                        actuallyPlan = actuallyPlan.Where(m => m.day_of_week == day).GroupBy(m => new { m.sku_id })
                                                          .Select(g => new PlanActually
                                                          {
                                                              sku_id = g.Key.sku_id,
                                                              plan_actually = g.Sum(m => m.plan_actually)
                                                          });
                        break;
                    case 5:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_fri)
                                                              });
                        actuallyPlan = actuallyPlan.Where(m => m.day_of_week == day).GroupBy(m => new { m.sku_id })
                                                             .Select(g => new PlanActually
                                                             {
                                                                 sku_id = g.Key.sku_id,
                                                                 plan_actually = g.Sum(m => m.plan_actually)
                                                             });
                        break;
                    case 6:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_sat)
                                                              });
                        actuallyPlan = actuallyPlan.Where(m => m.day_of_week == day).GroupBy(m => new { m.sku_id })
                                                           .Select(g => new PlanActually
                                                           {
                                                               sku_id = g.Key.sku_id,
                                                               plan_actually = g.Sum(m => m.plan_actually)
                                                           });
                        break;
                    case 7:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.sku_id, m.item.sku_name, m.item.sku_code })
                                                              .Select(g => new Plan
                                                              {
                                                                  Id = g.Key.sku_id,
                                                                  sku_code = g.Key.sku_code,
                                                                  sku_name = g.Key.sku_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_sun)
                                                              });
                        actuallyPlan = actuallyPlan.Where(m => m.day_of_week == day).GroupBy(m => new { m.sku_id })
                                                             .Select(g => new PlanActually
                                                             {
                                                                 sku_id = g.Key.sku_id,
                                                                 plan_actually = g.Sum(m => m.plan_actually)
                                                             });
                        break;
                    default:
                        // Handle invalid day value
                        return BadRequest("Invalid day value.");
                }

                plan = summedPlanDetail.ToList();


                if (actuallyPlan.ToList().Count() > 0)
                {
                    foreach (var n in plan)
                    {
                        var reason = reasonPlan.Where(m => m.day_of_week == day && m.sku_id == n.Id).FirstOrDefault();
                        var valueresult = actuallyPlan.Where(m => m.sku_id == n.Id).SingleOrDefault();
                        n.Actualvalue = valueresult.plan_actually;
                        n.reason = reason.reason_id;
                        checkAllSubmit = 1;
                    }
                }
                else
                {
                    foreach (var n in plan)
                    {


                        n.Actualvalue = n.value;
                       
                    }
                    checkAllSubmit = 2;
                }

               
            }
           

            ViewBag.checkAllSubmit = checkAllSubmit;


            return View(plan);
        }

        public ActionResult GetSkuDetail(int store, int day, int week, string type, int skuid, int valueInput)
        {
            var planDetailApprove = _context.PlanDetail.Include(m => m.item)
                                                       .Include(m => m.store).ThenInclude(m => m.store_type)
                                                       .Include(m => m.week)
                                                       .Where(m => m.approve == true);


            List<Plan> plan = new List<Plan>();
            // Sum plandetail by day with sku_name
            IQueryable<Plan> summedPlanDetail;
            if (day != 0 && store != 0 && week != 0 && (type != null && type != ""))
            {
                if (type == "Hub")
                {
                    planDetailApprove = planDetailApprove.Where(m => m.store_id == store && m.week_no == week && m.sku_id == skuid);

                }
                else
                {
                    var spoke = _context.StoreRelation.Where(m => m.store_hub_id == store).Select(m => m.store_spoke_id).ToList();
                    planDetailApprove = planDetailApprove.Where(m => spoke.Contains(m.store_id) && m.week_no == week && m.store.store_type.store_type_name == type && m.sku_id == skuid);

                }

                switch (day)
                {
                    case 1:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_mon)
                                                              });
                        break;
                    case 2:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_tues)
                                                              });
                        break;
                    case 3:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_wed)
                                                              });
                        break;
                    case 4:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_thu)
                                                              });
                        break;
                    case 5:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_fri)
                                                              });
                        break;
                    case 6:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_sat)
                                                              });
                        break;
                    case 7:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_sun)
                                                              });
                        break;
                    default:
                        // Handle invalid day value
                        return BadRequest("Invalid day value.");
                }



                plan = summedPlanDetail.ToList();
                var totalValue = summedPlanDetail.Sum(item => item.value);
                foreach (var item in plan)
                {
                    var ratio = (double)item.value / totalValue;
                    var newValue = (int)Math.Ceiling(ratio * valueInput); // นำ valueInput มาคูณด้วยอัตราส่วนและปัดขึ้น
                    item.value = newValue;
                }

                // หาค่าเกิน
                var excess = plan.Sum(item => item.value) - valueInput;
                if (excess > 0)
                {
                    // เรียงลำดับ plan โดยใช้ LINQ
                    var orderedPlan = plan.OrderBy(item => item.value);
                    // ลดค่าของ item.value ของตัวที่น้อยสุดลง 1
                    foreach (var item in orderedPlan)
                    {
                        if (excess <= 0)
                            break;

                        item.value--;
                        excess--;
                    }
                }
            }




            // ตัวอย่างข้อมูลที่จะส่งกลับ
            //var skuDetails = new List<object>
            //{
            //    new { store = "Store A", quantity = 10 },
            //    new { store = "Store B", quantity = 15 },
            //    new { store = "Store C", quantity = 20 }
            //};

            return Json(plan);
        }


        public List<Plan> GetPlan(int store, int day, int week, string type, int skuid, int valueInput)
        {
            var planDetailApprove = _context.PlanDetail.Include(m => m.item)
                                                       .Include(m => m.store).ThenInclude(m => m.store_type)
                                                       .Include(m => m.week)
                                                       .Where(m => m.approve == true);


            List<Plan> plan = new List<Plan>();
            // Sum plandetail by day with sku_name
            IQueryable<Plan> summedPlanDetail;
            if (day != 0 && store != 0 && week != 0 && (type != null && type != ""))
            {
                if (type == "Hub")
                {
                    planDetailApprove = planDetailApprove.Where(m => m.store_id == store && m.week_no == week && m.sku_id == skuid);

                }
                else
                {
                    var spoke = _context.StoreRelation.Where(m => m.store_hub_id == store).Select(m => m.store_spoke_id).ToList();
                    planDetailApprove = planDetailApprove.Where(m => spoke.Contains(m.store_id) && m.week_no == week && m.store.store_type.store_type_name == type && m.sku_id == skuid);

                }

                switch (day)
                {
                    case 1:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_mon)
                                                              });
                        break;
                    case 2:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_tues)
                                                              });
                        break;
                    case 3:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_wed)
                                                              });
                        break;
                    case 4:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_thu)
                                                              });
                        break;
                    case 5:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_fri)
                                                              });
                        break;
                    case 6:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_sat)
                                                              });
                        break;
                    case 7:
                        summedPlanDetail = planDetailApprove.GroupBy(m => new { m.store.store_code, m.store.store_name })
                                                              .Select(g => new Plan
                                                              {

                                                                  sku_code = g.Key.store_code,
                                                                  sku_name = g.Key.store_name, // Add sku_name
                                                                  value = g.Sum(m => m.plan_sun)
                                                              });
                        break;
                    default:
                        // Handle invalid day value
                        return plan;
                }



                plan = summedPlanDetail.ToList();
                var totalValue = summedPlanDetail.Sum(item => item.value);
                foreach (var item in plan)
                {
                    var ratio = (double)item.value / totalValue;
                    var newValue = (int)Math.Ceiling(ratio * valueInput); // นำ valueInput มาคูณด้วยอัตราส่วนและปัดขึ้น
                    item.value = newValue;
                }

                // หาค่าเกิน
                var excess = plan.Sum(item => item.value) - valueInput;
                if (excess > 0)
                {
                    // เรียงลำดับ plan โดยใช้ LINQ
                    var orderedPlan = plan.OrderBy(item => item.value);
                    // ลดค่าของ item.value ของตัวที่น้อยสุดลง 1
                    foreach (var item in orderedPlan)
                    {
                        if (excess <= 0)
                            break;

                        item.value--;
                        excess--;
                    }
                }
            }




            // ตัวอย่างข้อมูลที่จะส่งกลับ
            //var skuDetails = new List<object>
            //{
            //    new { store = "Store A", quantity = 10 },
            //    new { store = "Store B", quantity = 15 },
            //    new { store = "Store C", quantity = 20 }
            //};

            return plan;
        }

     



        // POST: PlanActuallies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> SubmitAll(string json, string type)
        {
            var jsonData = JsonConvert.DeserializeObject<List<PlanActually>>(json);

            foreach (var n in jsonData)
            {

                var PlanActually = _context.PlanActually.Where(m => m.week_no == n.week_no && m.sku_id == n.sku_id && m.day_of_week == n.day_of_week);
                if (type == "Hub")
                {
                    PlanActually = PlanActually.Where(m => m.store_id == n.store_id);
                    var checkUpate = PlanActually.SingleOrDefault();
                    if (checkUpate != null)
                    {

                        var result = GetPlan(n.store_id, n.day_of_week, n.week_no, type, n.sku_id, n.plan_actually);

                        foreach (var z in result)
                        {
                            var getstoreId = _context.Store.Where(x => x.store_code == z.sku_code).Select(x => x.id).SingleOrDefault();


                            checkUpate.plan_actually = z.value;
                            checkUpate.reason_id = n.reason_id;


                            _context.Update(checkUpate);

                        }


                    }
                    else
                    {
                        var result = GetPlan(n.store_id, n.day_of_week, n.week_no, type, n.sku_id, n.plan_actually);
                        foreach (var z in result)
                        {
                            var getstoreId = _context.Store.Where(x => x.store_code == z.sku_code).Select(x => x.id).SingleOrDefault();
                            PlanActually plan = new PlanActually();
                            plan = n;
                            plan.store_id = getstoreId;
                            plan.plan_actually = z.value;



                            _context.Add(plan);

                        }


                    }

                }
                else
                {

                    var result = GetPlan(n.store_id, n.day_of_week, n.week_no, type, n.sku_id, n.plan_actually);



                    foreach (var z in result)
                    {
                      
                        var getstoreId = _context.Store.Where(x => x.store_code == z.sku_code).Select(x => x.id).SingleOrDefault();
                      
                        var PlanActuallySpoke = _context.PlanActually.Where(m => m.week_no == n.week_no && m.sku_id == n.sku_id && m.day_of_week == n.day_of_week && m.store_id == getstoreId).SingleOrDefault();
                        var checkUpdate = PlanActuallySpoke;
                        if (checkUpdate != null)
                        {
                            checkUpdate.plan_actually = z.value;
                            checkUpdate.reason_id = n.reason_id;
                            checkUpdate.plan_value = n.plan_value;


                            _context.Update(checkUpdate);
                        }
                        else
                        {
                            PlanActually plan = new PlanActually();
                            plan.sku_id = n.sku_id;
                            plan.week_no = n.week_no;
                            plan.day_of_week = n.day_of_week;
                            plan.store_id = getstoreId;
                            plan.plan_actually = z.value;
                            plan.plan_value = n.plan_value;
                            plan.approve = 1;



                            _context.Add(plan);
                        }


                      

                    }

                  


                }


            }




            await _context.SaveChangesAsync();

            var returnData = jsonData.FirstOrDefault();
            return RedirectToAction("Index", new { storeID = returnData.store_id, week = returnData.week_no, day = returnData.day_of_week, cycle = type });
        }



 
    }
}
