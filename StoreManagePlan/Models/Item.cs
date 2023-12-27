using System.ComponentModel.DataAnnotations;

namespace StoreManagePlan.Models
{
    public class Item
    {
        [Key]
        public Int32 ID { get; set; }

        public String SKU_CODE { get; set; }
        public String SKU_NAME { get; set; }
    }
}
