using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagePlan.Models
{
    public class PlanDetail
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("item")]
        public int sku_id { get; set; }

        [ForeignKey("store")]
        public int store_id { get; set; }

        public int plan_mon { get; set; }
        public int plan_tues { get; set; }
        public int plan_wed { get; set; }
        public int plan_thu { get; set; }
        public int plan_fri { get; set; }
        public int plan_sat { get; set; }
        public int plan_sun { get; set; }

        [ForeignKey("week")]
        public int week_no { get; set; }
       
        public Store store { get; set; }
        public Item item { get; set; }
        public Week week { get; set; }
    
    }
}
