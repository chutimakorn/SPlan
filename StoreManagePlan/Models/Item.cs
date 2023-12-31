using System.ComponentModel.DataAnnotations;

namespace StoreManagePlan.Models
{
    public class Item
    {
        [Key]
        public Int32 id { get; set; }

        public String sku_code { get; set; }
        public String sku_name { get; set; }
        public String create_date { get; set; }
        public String? update_date { get; set; }
        public String? effective_date { get; set; }
    }
}
