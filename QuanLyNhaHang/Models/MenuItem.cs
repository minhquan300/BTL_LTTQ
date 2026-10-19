namespace QuanLyNhaHang.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class MenuItem
    {
        public int MenuItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public bool IsAvailable { get; set; }

        public string PriceDisplay => Price.ToString("N0") + " đ";
    }
}
