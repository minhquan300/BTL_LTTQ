namespace QuanLyNhaHang.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Tien mat";
        public string PaymentStatus { get; set; } = "Thanh cong";
        public DateTime PaymentTime { get; set; }

        public string MethodDisplay => PaymentMethod switch
        {
            "Tien mat" => "Tiền mặt",
            "Chuyen khoan" => "Chuyển khoản",
            "The" or "Card" => "Thẻ",
            _ => PaymentMethod
        };
    }
}
