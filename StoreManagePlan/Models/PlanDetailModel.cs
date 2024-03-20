using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreManagePlan.Models
{
    public class PlanDetailModel
    {
        public string? sku_code { get; set; }
        public string? sku_name { get; set; }
        public int store_id { get; set; }

        public int plan_mon { get; set; }
        public int plan_tues { get; set; }
        public int plan_wed { get; set; }
        public int plan_thu { get; set; }
        public int plan_fri { get; set; }
        public int plan_sat { get; set; }
        public int plan_sun { get; set; }
        public string type { get; set; }
    }
}
