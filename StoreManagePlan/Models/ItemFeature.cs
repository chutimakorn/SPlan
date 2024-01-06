using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagePlan.Models
{
    public class ItemFeature
    {


        [Key, ForeignKey("Store")]
        public Int32 store_id { get; set; }

        [Key, ForeignKey("Item")]
        public Int32 item_id { get; set; }

        public Int32 minimum_feature { get; set; }
        public Int32 maximum_feature { get; set; }
        public Int32 default_feature { get; set; }
        public String? create_date { get; set; }
        public String? update_date { get; set; }
    

        public  Store Store  { get; set; }
        public  Item Item  { get; set; }
    }
}
