namespace QuanLyNhaHang.Models;

public sealed class PaidInvoiceSummary
{
    public int OrderID { get; init; }
    public string OrderCode { get; init; } = string.Empty;
    public DateTime PaymentTime { get; init; }
    public string TableName { get; init; } = string.Empty;
    public string CashierName { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public decimal Amount { get; init; }

    public string PaymentMethodDisplay => PaymentMethod switch
    {
        "Tien mat" => "Tiền mặt",
        "Chuyen khoan" => "Chuyển khoản",
        "The" or "Card" => "Thẻ",
        _ => PaymentMethod
    };
}
