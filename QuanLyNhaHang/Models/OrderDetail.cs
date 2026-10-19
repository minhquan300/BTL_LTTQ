namespace QuanLyNhaHang.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public string TableName { get; set; } = string.Empty;
        public int MenuItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemStatus { get; set; } = "Moi";
        public DateTime OrderedAt { get; set; }
        public DateTime? CookingStartedAt { get; set; }
        public DateTime? CookingCompletedAt { get; set; }
        public DateTime? ServedAt { get; set; }
        public string? Note { get; set; }

        public decimal SubTotal => Quantity * UnitPrice;

        public string StatusDisplay => ItemStatus switch
        {
            "Moi" => "Mới",
            "Cho bep" => "Chờ bếp",
            "Dang che bien" => "Đang chế biến",
            "San sang" => "Sẵn sàng",
            "Da phuc vu" => "Đã phục vụ",
            "Da huy" => "Đã hủy",
            _ => ItemStatus
        };

        public Color StatusColor => ItemStatus switch
        {
            "Moi" => Color.FromArgb(107, 114, 128),
            "Cho bep" => Color.FromArgb(245, 158, 11),
            "Dang che bien" => Color.FromArgb(230, 126, 34),
            "San sang" => Color.FromArgb(39, 174, 96),
            "Da phuc vu" => Color.FromArgb(127, 140, 141),
            "Da huy" => Color.FromArgb(231, 76, 60),
            _ => Color.Gray
        };

        /// <summary>Thời gian chờ chế biến (phút)</summary>
        public double WaitTimeMinutes
        {
            get
            {
                var end = CookingStartedAt ?? (ItemStatus is "Moi" or "Cho bep" ? DateTime.Now : OrderedAt);
                return Math.Max(0, (end - OrderedAt).TotalMinutes);
            }
        }

        /// <summary>Thời gian nấu / chế biến (phút)</summary>
        public double CookingTimeMinutes
        {
            get
            {
                if (!CookingStartedAt.HasValue) return 0;
                var end = CookingCompletedAt ?? DateTime.Now;
                return Math.Max(0, (end - CookingStartedAt.Value).TotalMinutes);
            }
        }

        /// <summary>Thời gian chờ bưng ra bàn sau khi nấu xong (phút)</summary>
        public double DeliveryTimeMinutes
        {
            get
            {
                if (!CookingCompletedAt.HasValue) return 0;
                var end = ServedAt ?? DateTime.Now;
                return Math.Max(0, (end - CookingCompletedAt.Value).TotalMinutes);
            }
        }

        /// <summary>Tổng thời gian phục vụ món (phút)</summary>
        public double TotalServiceMinutes
        {
            get
            {
                var end = ServedAt ?? DateTime.Now;
                return Math.Max(0, (end - OrderedAt).TotalMinutes);
            }
        }

        /// <summary>Đánh giá tốc độ phục vụ: Nhanh (<10p), Chuẩn (10-15p), Chậm (>15p)</summary>
        public string ServiceSpeedRating
        {
            get
            {
                var mins = TotalServiceMinutes;
                if (mins <= 10) return "Nhanh";
                if (mins <= 20) return "Đúng hẹn";
                return "Chậm trễ";
            }
        }

        public Color ServiceSpeedColor
        {
            get
            {
                var mins = TotalServiceMinutes;
                if (mins <= 10) return Color.FromArgb(39, 174, 96);
                if (mins <= 20) return Color.FromArgb(243, 156, 18);
                return Color.FromArgb(231, 76, 60);
            }
        }
    }
}
