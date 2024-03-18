using System.ComponentModel.DataAnnotations;

namespace StoreManagePlan.Models
{
    public class Reason
    {
        [Key]
        public int id { get; set; }

        public String reason { get; set; }
     
    }
}
