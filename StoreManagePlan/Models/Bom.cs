using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagePlan.Models
{
    public class Bom
    {
        [Key, ForeignKey("Item")]
        public Int32 sku_id { get; set; }

        [Key]
        public String ingredient_sku { get; set; }
        public String? ingredient_name { get; set; }

        public Int32? min_batch_hub { get; set; }
        public Int32? min_batch_non_hub { get; set; }

        public String? batch_uom { get; set; }

        public Double? weight_hub { get; set; }
        public String? weight_uom { get; set; }

        public String create_date { get; set; }
        public String? update_date { get; set; }

        public Item Item { get; set; }

    }
}
