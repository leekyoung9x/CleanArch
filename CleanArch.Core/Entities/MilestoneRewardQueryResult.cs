namespace CleanArch.Core.Entities
{
    public class MilestoneRewardQueryResult
    {
        public int Id { get; set; }
        public string MilestoneName { get; set; }
        public long Amount { get; set; }
        public int Claimed { get; set; }
        public int Claimable { get; set; }
        public int Current { get; set; }
        public int? ItemId { get; set; }
        public int? Quantity { get; set; }
        public string Options { get; set; }
        public string ItemName { get; set; }
        public int? ItemIconId { get; set; }
        public int? ItemType { get; set; }
        public string ItemDescription { get; set; }
    }
}
