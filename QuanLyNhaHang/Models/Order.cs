namespace QuanLyNhaHang.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int TableID { get; set; }
        public string TableName { get; set; } = string.Empty;
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime OrderTime { get; set; }
        public string OrderStatus { get; set; } = "Moi";
        public List<OrderDetail> Details { get; set; } = new();

        public decimal TotalAmount => Details.Sum(d => d.Quantity * d.UnitPrice);

        public string StatusDisplay => OrderStatus switch
        {
            "Moi" => "Mới",
            "Dang xu ly" => "Đang xử lý",
            "Hoan thanh" => "Hoàn thành",
            "Da thanh toan" => "Đã thanh toán",
            "Da huy" => "Đã hủy",
            _ => OrderStatus
        };
    }
}
