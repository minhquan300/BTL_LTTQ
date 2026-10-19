/*
   Optional, manual data correction for existing category rows.
   This file is not executed by the application or build.
*/
SET XACT_ABORT ON;

BEGIN TRANSACTION;

UPDATE dbo.Categories
SET CategoryName = N'Khai vị', Description = N'Các món khai vị'
WHERE CategoryName = N'Khai vi';

UPDATE dbo.Categories
SET CategoryName = N'Gỏi và nộm', Description = N'Gỏi, nộm và món trộn'
WHERE CategoryName = N'Goi va nom';

UPDATE dbo.Categories
SET CategoryName = N'Món gà', Description = N'Các món chế biến từ gà'
WHERE CategoryName = N'Mon ga';

UPDATE dbo.Categories
SET CategoryName = N'Món bò', Description = N'Các món chế biến từ bò'
WHERE CategoryName = N'Mon bo';

UPDATE dbo.Categories
SET CategoryName = N'Món heo', Description = N'Các món chế biến từ heo'
WHERE CategoryName = N'Mon heo';

UPDATE dbo.Categories
SET CategoryName = N'Hải sản', Description = N'Tôm, mực, cá và hải sản'
WHERE CategoryName = N'Hai san';

UPDATE dbo.Categories
SET CategoryName = N'Rau và đậu', Description = N'Rau xào và món đậu'
WHERE CategoryName = N'Rau va dau';

UPDATE dbo.Categories
SET CategoryName = N'Cơm', Description = N'Các món cơm'
WHERE CategoryName = N'Com';

UPDATE dbo.Categories
SET CategoryName = N'Mì và bún', Description = N'Mì, bún và phở'
WHERE CategoryName = N'Mi va bun';

UPDATE dbo.Categories
SET CategoryName = N'Lẩu', Description = N'Các loại lẩu'
WHERE CategoryName = N'Lau';

UPDATE dbo.Categories
SET CategoryName = N'Tráng miệng', Description = N'Trái cây và món ngọt'
WHERE CategoryName = N'Trang mieng';

UPDATE dbo.Categories
SET CategoryName = N'Đồ uống', Description = N'Trà, nước ép và nước giải khát'
WHERE CategoryName = N'Do uong';

COMMIT TRANSACTION;
