using System.ComponentModel.DataAnnotations;

namespace StoreManagePlan.Models
{
    public class Week
    {
        [Key]
        public int week_no { get; set; }

        public String start_date { get; set; }
        public String end_date { get; set; }
        public int status { get; set; }
       
    }
}
