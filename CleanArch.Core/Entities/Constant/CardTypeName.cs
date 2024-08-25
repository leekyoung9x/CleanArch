namespace CleanArch.Core.Entities.Constant
{
    public static class CardTypeName
    {
        public static string Success = "Nạp thẻ thành công";
        public static string SuccessHalf = "Thẻ thành công sai mệnh giá";
        public static string Error = "Thẻ lỗi hãy gửi lại";
        public static string Maintain = "Nạp thẻ đang bảo trì";
        public static string Pending = "Gửi thẻ thành công hãy đợi 5s để duyệt thẻ";
        public static string SendFail = "Gửi thẻ thất bại - Có lý do đi kèm ở phần thông báo trả về";
    }
}
