using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagePlan.Models
{
    public class StoreRelation
    {
        [Key]
        public Int32 id { get; set; }

        [ForeignKey("StoreHub")]
        public Int32 store_hub_id { get; set; }

        [ForeignKey("StoreSpoke")]
        public Int32 store_spoke_id { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string create_date { get; set; }
        public string update_date { get; set; }

        public Store StoreHub { get; set; }
        public Store StoreSpoke { get; set; }
    }
}
