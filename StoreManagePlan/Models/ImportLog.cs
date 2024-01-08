using System.ComponentModel.DataAnnotations;

namespace StoreManagePlan.Models
{
    public class ImportLog
    {
        [Key]
        public Int32 id { get; set; }
        public string? current_name { get; set; }
        public string? old_name { get; set; }
        public string? menu { get; set; }
        public string? message { get; set; }
        public string? create_date { get; set; }
        public string? status { get; set;}
    }
}
