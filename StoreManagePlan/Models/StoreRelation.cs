namespace StoreManagePlan.Models
{
    public class StoreRelation
    {

        [Key, ForeignKey("StoreHub")]
        public Int32 store_hub_id { get; set; }

        [Key, ForeignKey("StoreSpoke")]
        public Int32 store_spoke_id { get; set; }
        public Int32 start_date { get; set; }
        public Int32 end_date { get; set; }
        public Int32 create_date { get; set; }
        public Int32 update_date { get; set; }

        public Store StoreHub { get; set; }
        public Store StoreSpoke { get; set; }
    }
}
