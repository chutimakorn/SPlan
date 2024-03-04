using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagePlan.Models
{
    public class Store
    {
        [Key]
        public Int32 id { get; set; }

        [ForeignKey("store_type")]
        public Int32 type_id { get; set; }

        public String store_code { get; set; }
        public String store_name { get; set; }
        public String create_date { get; set; }
        public String update_date { get; set; }
        public String start_date { get; set; }
        public String end_date { get; set; }
      
        
        public  StoreType store_type { get; set; }
    }
}
