using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagePlan.Models
{
    public class PlanActually
    {
     

        [Key]
        [ForeignKey("store")]
        public int store_id { get; set; }
        [Key]
        [ForeignKey("item")]
        public int sku_id { get; set; }

        [Key]
        [ForeignKey("week")]
        public int week_no { get; set; }
        [Key]
        public int day_of_week { get; set; }
       
        public int plan_value { get; set; }
        public int plan_actually { get; set; }

        [ForeignKey("reason")]
        public int reason_id { get; set; }
        public int approve { get; set; }




        public Store store { get; set; }
        public Week week { get; set; }
        public Item  item { get; set; }
        public Reason reason { get; set; }

    }
}


