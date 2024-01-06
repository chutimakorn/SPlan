using System.ComponentModel.DataAnnotations;

namespace StoreManagePlan.Models
{
    public class StoreType
    {
        [Key]
        public Int32 id { get; set; }

        public String store_type_code { get; set; }
        public String store_type_name { get; set; }
        public String create_date { get; set; }
        public String? update_date { get; set; }
      
    }
}
