namespace StoreManagePlan.Models
{
    public class DailyProductModel
    {
        public string ProductName { get; set; }
        public int HubAmt { get; set; }
        public int SpokeAmt { get; set; }
        public int TotalAmt { get; set; }
        public string Unit { get; set; }
    }
}
