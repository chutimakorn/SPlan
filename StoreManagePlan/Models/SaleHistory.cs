using System.ComponentModel.DataAnnotations;

namespace StoreManagePlan.Models
{
    public class SaleHistory
    {
        [Key]
        public int id { get; set; }

        public string store_code { get; set; }
        public string sku_code { get; set; }
        public int day_of_week { get; set; }
        public int qty_base { get; set; }
        public int qty_bad { get; set; }
        public int qty_rtc { get; set; }
        public int qty_kl { get; set; }
        public double bad_percent { get; set; }
        public double rtc_percent { get; set; }
        public double kl_percent { get; set; }
        public int week_no { get; set; }
      
    }
}
