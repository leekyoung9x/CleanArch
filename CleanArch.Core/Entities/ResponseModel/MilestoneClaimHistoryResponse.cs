namespace CleanArch.Core.Entities.ResponseModel
{
    public class MilestoneClaimHistoryResponse
    {
        public int MilestoneId { get; set; }
        public string MilestoneName { get; set; }
        public long RequiredScore { get; set; }
        public string RewardPackageName { get; set; }
        public DateTime ClaimedAt { get; set; }
        public string GiftCode { get; set; }
        public long? GiftCodeId { get; set; }
    }
}
