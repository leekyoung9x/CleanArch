namespace CleanArch.Core.Entities.ResponseModel
{
    public class MilestoneRewardResponse
    {
        public int Id { get; set; }
        public long Amount { get; set; }
        public bool Claimed { get; set; }
        public bool Claimable { get; set; }
        public bool Current { get; set; }
        public List<MilestoneItemResponse> Items { get; set; }

        public MilestoneRewardResponse()
        {
            Items = new List<MilestoneItemResponse>();
        }
    }

    public class MilestoneItemResponse
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public int Qty { get; set; }
        public string Name { get; set; }
        public List<ItemStatResponse> Stats { get; set; }

        public MilestoneItemResponse()
        {
            Stats = new List<ItemStatResponse>();
        }
    }

    public class ItemStatResponse
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class RewardPackageContentOption
    {
        public int Id { get; set; }
        public object Value { get; set; }
    }
}
