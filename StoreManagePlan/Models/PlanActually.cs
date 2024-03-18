using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagePlan.Models
{
    public class PlanActually
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("planDetail")]
        public int plan_detail_id { get; set; }

        public int day_of_week { get; set; }
       
        public int plan_actually { get; set; }

        [ForeignKey("reason")]
        public int reason_id { get; set; }
        public int approve { get; set; }




        public PlanDetail planDetail { get; set; }
        public Reason reason { get; set; }

    }
}
