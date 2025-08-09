namespace CleanArch.Core.Entities
{
    public class GiftCodeItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public List<GiftCodeOption>? Options { get; set; } = new List<GiftCodeOption>();
    }

    public class GiftCodeOption
    {
        public int Id { get; set; }
        public int Param { get; set; }
    }
}
