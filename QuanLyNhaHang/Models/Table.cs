namespace QuanLyNhaHang.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string? QRCode { get; set; }
        public int Capacity { get; set; }
        public string TableStatus { get; set; } = "Trong";

        // Display helpers
        public Color StatusColor => TableStatus switch
        {
            "Trong" => Color.FromArgb(39, 174, 96),
            "Dang phuc vu" => Color.FromArgb(231, 76, 60),
            "Dat truoc" => Color.FromArgb(241, 196, 15),
            "Ngung su dung" => Color.FromArgb(127, 140, 141),
            _ => Color.Gray
        };

        public string StatusDisplay => TableStatus switch
        {
            "Trong" => "Trống",
            "Dang phuc vu" => "Đang phục vụ",
            "Dat truoc" => "Đặt trước",
            "Ngung su dung" => "Ngưng sử dụng",
            _ => TableStatus
        };
    }
}
