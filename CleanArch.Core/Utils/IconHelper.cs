namespace CleanArch.Core.Utils
{
    public static class IconHelper
    {
        private static readonly Dictionary<int, string> IconMapping = new Dictionary<int, string>
        {
            { 0, "👕" },    // Áo
            { 1, "👖" },    // Quần
            { 2, "🧤" },    // Găng tay
            { 3, "👟" },    // Giày
            { 4, "📡" },    // Rada
            { 5, "🧥" },    // Áo choàng
            { 6, "💍" },    // Nhẫn
            { 7, "📿" },    // Chuỗi
            { 8, "⚔️" },    // Vũ khí
            { 9, "💊" },    // Vật phẩm tiêu hao
            { 10, "🪙" },   // Vàng
            { 11, "🎁" },   // Hộp quà
            { 12, "💎" },   // Ngọc
            
            // Có thể thêm nhiều icon khác dựa trên icon_id trong database
            { 100, "🎴" },  // Thẻ 5 Sao
            { 101, "🪙" },  // Vàng
            { 102, "📦" },  // Gói Quà Đặc Biệt
            { 103, "💎" },  // Mảnh Ghép Kim Cương
        };

        public static string GetIconByIconId(int iconId)
        {
            return IconMapping.TryGetValue(iconId, out var icon) ? icon : "📦";
        }

        public static string GetIconByItemType(int itemType)
        {
            // Fallback mapping based on item type if specific icon_id not found
            return itemType switch
            {
                0 => "👕",   // Áo
                1 => "👖",   // Quần
                2 => "🧤",   // Găng tay
                3 => "👟",   // Giày
                4 => "📡",   // Rada
                5 => "🧥",   // Áo choàng
                6 => "💍",   // Nhẫn
                7 => "📿",   // Chuỗi
                8 => "⚔️",   // Vũ khí
                9 => "💊",   // Vật phẩm tiêu hao
                10 => "🪙",  // Vàng
                11 => "🎁",  // Hộp quà
                12 => "💎",  // Ngọc
                _ => "📦"
            };
        }
    }
}
