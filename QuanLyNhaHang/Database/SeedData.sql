/*
    SEED DATA - QUAN LY NHA HANG
    Du lieu mau phuc vu kiem thu.
    Luu y:
    - Chi chay sau khi da tao database bang QLBH.sql.
    - Script nay yeu cau cac bang dang rong de tranh trung lap ID.
    - PasswordHash tam thoi la chuoi danh dau, chua dung de dang nhap.
*/
USE [QuanLyNhaHang];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS (SELECT 1 FROM dbo.Roles)
       OR EXISTS (SELECT 1 FROM dbo.Employees)
       OR EXISTS (SELECT 1 FROM dbo.Categories)
       OR EXISTS (SELECT 1 FROM dbo.MenuItems)
       OR EXISTS (SELECT 1 FROM dbo.DiningTables)
       OR EXISTS (SELECT 1 FROM dbo.Orders)
       OR EXISTS (SELECT 1 FROM dbo.OrderDetails)
       OR EXISTS (SELECT 1 FROM dbo.Payments)
       OR EXISTS (SELECT 1 FROM dbo.OrderStatusHistory)
       OR EXISTS (SELECT 1 FROM dbo.OrderDetailStatusHistory)
    BEGIN
        ;THROW 50001, N'Cac bang khong rong. Hay dung database moi hoac kiem tra du lieu truoc khi chay SeedData.sql.', 1;
    END;

    /* 1. ROLES */
    SET IDENTITY_INSERT dbo.Roles ON;
    INSERT INTO dbo.Roles (RoleID, RoleName, Description) VALUES
    (1, N'Quan ly', N'Quan ly hoat dong nha hang'),
    (2, N'Thu ngan', N'Tiep nhan va thanh toan don hang'),
    (3, N'Phuc vu', N'Tiep nhan yeu cau va phuc vu khach'),
    (4, N'Dau bep', N'Che bien mon an');
    SET IDENTITY_INSERT dbo.Roles OFF;

    /* 2. EMPLOYEES
       PasswordHash tam thoi. Can thay bang hash dung sau khi nhom hoan thien AuthService. */
    SET IDENTITY_INSERT dbo.Employees ON;
    INSERT INTO dbo.Employees
        (EmployeeID, Username, PasswordHash, FullName, Phone, IsActive, RoleID, CreatedAt)
    VALUES
    (1, 'admin', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Nguyen Van An', '0901000001', 1, 1, DATEADD(day,-180, SYSDATETIME())),
    (2, 'manager01', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Tran Thi Bich', '0901000002', 1, 1, DATEADD(day,-160, SYSDATETIME())),
    (3, 'cashier01', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Le Van Cuong', '0901000003', 1, 2, DATEADD(day,-150, SYSDATETIME())),
    (4, 'cashier02', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Pham Thi Dung', '0901000004', 1, 2, DATEADD(day,-140, SYSDATETIME())),
    (5, 'cashier03', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Hoang Van Giang', '0901000005', 1, 2, DATEADD(day,-130, SYSDATETIME())),
    (6, 'service01', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Vu Thi Hoa', '0901000006', 1, 3, DATEADD(day,-120, SYSDATETIME())),
    (7, 'service02', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Do Van Hung', '0901000007', 1, 3, DATEADD(day,-115, SYSDATETIME())),
    (8, 'service03', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Bui Thi Lan', '0901000008', 1, 3, DATEADD(day,-110, SYSDATETIME())),
    (9, 'service04', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Ngo Van Minh', '0901000009', 1, 3, DATEADD(day,-105, SYSDATETIME())),
    (10, 'service05', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Dang Thi Ngoc', '0901000010', 1, 3, DATEADD(day,-100, SYSDATETIME())),
    (11, 'chef01', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Phan Van Phuc', '0901000011', 1, 4, DATEADD(day,-95, SYSDATETIME())),
    (12, 'chef02', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Ho Thi Quyen', '0901000012', 1, 4, DATEADD(day,-90, SYSDATETIME())),
    (13, 'chef03', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Nguyen Duc Son', '0901000013', 1, 4, DATEADD(day,-85, SYSDATETIME())),
    (14, 'chef04', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Tran Van Tuan', '0901000014', 1, 4, DATEADD(day,-80, SYSDATETIME())),
    (15, 'service06', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Le Thi Yen', '0901000015', 1, 3, DATEADD(day,-75, SYSDATETIME()));
    SET IDENTITY_INSERT dbo.Employees OFF;

    /* 3. CATEGORIES */
    SET IDENTITY_INSERT dbo.Categories ON;
    INSERT INTO dbo.Categories (CategoryID, CategoryName, Description, IsActive) VALUES
    (1, N'Khai vi', N'Cac mon khai vi', 1),
    (2, N'Goi va nom', N'Goi, nom va mon tron', 1),
    (3, N'Mon ga', N'Cac mon che bien tu ga', 1),
    (4, N'Mon bo', N'Cac mon che bien tu bo', 1),
    (5, N'Mon heo', N'Cac mon che bien tu heo', 1),
    (6, N'Hai san', N'Tom, muc, ca va hai san', 1),
    (7, N'Rau va dau', N'Rau xao va mon dau', 1),
    (8, N'Com', N'Cac mon com', 1),
    (9, N'Mi va bun', N'Mi, bun va pho', 1),
    (10, N'Lau', N'Cac loai lau', 1),
    (11, N'Trang mieng', N'Trai cay va mon ngot', 1),
    (12, N'Do uong', N'Tra, nuoc ep va nuoc giai khat', 1);
    SET IDENTITY_INSERT dbo.Categories OFF;

    /* 4. MENU ITEMS - 100 mon, moi danh muc co nhieu mon */
    SET IDENTITY_INSERT dbo.MenuItems ON;
    INSERT INTO dbo.MenuItems
        (MenuItemID, ItemName, CategoryID, Price, Description, ImagePath, IsAvailable)
    VALUES
    (1,N'Khoai tay chien',1,45000,N'Khoai tay chien gion',NULL,1),
    (2,N'Nem ran',1,55000,N'Nem ran truyen thong',NULL,1),
    (3,N'Cha gio hai san',1,65000,N'Cha gio nhan hai san',NULL,1),
    (4,N'Canh ga chien nuoc mam',1,79000,N'Canh ga chien nuoc mam',NULL,1),
    (5,N'Banh xep chien',1,50000,N'Banh xep chien gion',NULL,1),
    (6,N'Goi ngo sen tom thit',2,85000,N'Goi ngo sen tom thit',NULL,1),
    (7,N'Goi bo rau thom',2,89000,N'Goi bo rau thom',NULL,1),
    (8,N'Nom du du bo kho',2,75000,N'Nom du du bo kho',NULL,1),
    (9,N'Goi ga xe phay',2,79000,N'Goi ga xe phay',NULL,1),
    (10,N'Goi xoai tom',2,82000,N'Goi xoai xanh tom tuoi',NULL,1),
    (11,N'Ga nuong mat ong',3,159000,N'Ga nuong mat ong',NULL,1),
    (12,N'Ga hap la chanh',3,149000,N'Ga hap la chanh',NULL,1),
    (13,N'Ga rang muoi',3,169000,N'Ga rang muoi',NULL,1),
    (14,N'Ga sot me',3,155000,N'Ga chien sot me',NULL,1),
    (15,N'Canh ga nuong sa',3,99000,N'Canh ga nuong sa',NULL,1),
    (16,N'Bo luc lac',4,169000,N'Bo luc lac khoai tay',NULL,1),
    (17,N'Bo xao luc lac',4,159000,N'Bo xao rau cu',NULL,1),
    (18,N'Bo bit tet',4,189000,N'Bo bit tet sot tieu',NULL,1),
    (19,N'Bo sot vang',4,149000,N'Bo sot vang an kem banh mi',NULL,1),
    (20,N'Bo nuong la lot',4,139000,N'Bo nuong la lot',NULL,1),
    (21,N'Suon heo nuong',5,149000,N'Suon heo nuong mat ong',NULL,1),
    (22,N'Suon xao chua ngot',5,139000,N'Suon xao chua ngot',NULL,1),
    (23,N'Ba chi rang chay canh',5,119000,N'Ba chi rang chay canh',NULL,1),
    (24,N'Thịt kho trung',5,99000,N'Thit kho trung',NULL,1),
    (25,N'Heo quay',5,159000,N'Heo quay da gion',NULL,1),
    (26,N'Tom rang muoi',6,179000,N'Tom rang muoi',NULL,1),
    (27,N'Muc chien gion',6,139000,N'Muc chien gion',NULL,1),
    (28,N'Muc xao sa ot',6,149000,N'Muc xao sa ot',NULL,1),
    (29,N'Ca chien xu',6,159000,N'Ca chien xu',NULL,1),
    (30,N'Tom hap bia',6,189000,N'Tom hap bia',NULL,1),
    (31,N'Rau muong xao toi',7,55000,N'Rau muong xao toi',NULL,1),
    (32,N'Cai thia xao nam',7,65000,N'Cai thia xao nam',NULL,1),
    (33,N'Rau cu luoc kho quat',7,59000,N'Rau cu luoc kho quat',NULL,1),
    (34,N'Dau hu sot ca',7,69000,N'Dau hu sot ca',NULL,1),
    (35,N'Nam xao bo',7,79000,N'Nam xao bo',NULL,1),
    (36,N'Com trang',8,15000,N'Com trang phan',NULL,1),
    (37,N'Com chien hai san',8,89000,N'Com chien hai san',NULL,1),
    (38,N'Com chien duong chau',8,79000,N'Com chien duong chau',NULL,1),
    (39,N'Com ga nuong',8,85000,N'Com ga nuong',NULL,1),
    (40,N'Com suon nuong',8,89000,N'Com suon nuong',NULL,1),
    (41,N'Pho bo tai',9,75000,N'Pho bo tai',NULL,1),
    (42,N'Pho bo chin',9,75000,N'Pho bo chin',NULL,1),
    (43,N'Bun cha Ha Noi',9,79000,N'Bun cha Ha Noi',NULL,1),
    (44,N'Bun bo Hue',9,79000,N'Bun bo Hue',NULL,1),
    (45,N'Mi xao bo',9,85000,N'Mi xao bo rau cai',NULL,1),
    (46,N'Lau thai hai san',10,329000,N'Lau thai hai san 2-3 nguoi',NULL,1),
    (47,N'Lau ga la e',10,299000,N'Lau ga la e',NULL,1),
    (48,N'Lau bo nhung dam',10,349000,N'Lau bo nhung dam',NULL,1),
    (49,N'Lau nam',10,279000,N'Lau nam rau cu',NULL,1),
    (50,N'Lau ca',10,299000,N'Lau ca chua cay',NULL,1),
    (51,N'Che khuc bach',11,45000,N'Che khuc bach',NULL,1),
    (52,N'Rau cau dua',11,35000,N'Rau cau dua tuoi',NULL,1),
    (53,N'Banh flan',11,30000,N'Banh flan caramel',NULL,1),
    (54,N'Trai cay theo mua',11,59000,N'Dia trai cay theo mua',NULL,1),
    (55,N'Kem tuoi',11,39000,N'Kem tuoi nhieu vi',NULL,1),
    (56,N'Tra da',12,10000,N'Tra da',NULL,1),
    (57,N'Tra chanh',12,25000,N'Tra chanh tuoi',NULL,1),
    (58,N'Tra dao',12,35000,N'Tra dao cam sa',NULL,1),
    (59,N'Nuoc ep cam',12,39000,N'Nuoc ep cam tuoi',NULL,1),
    (60,N'Nuoc ep dua hau',12,35000,N'Nuoc ep dua hau',NULL,1),
    (61,N'Bap rang bo',1,35000,N'Bap rang bo',NULL,1),
    (62,N'Xuc xich chien',1,45000,N'Xuc xich chien',NULL,1),
    (63,N'Pho mai que',1,49000,N'Pho mai que',NULL,1),
    (64,N'Banh mi bo toi',1,39000,N'Banh mi bo toi',NULL,1),
    (65,N'Chao tom',1,69000,N'Chao tom nuong',NULL,1),
    (66,N'Goi rau tien vua',2,69000,N'Goi rau tien vua',NULL,1),
    (67,N'Goi cu hu dua',2,79000,N'Goi cu hu dua tom thit',NULL,1),
    (68,N'Goi ca mai',2,99000,N'Goi ca mai',NULL,1),
    (69,N'Nom tai heo',2,75000,N'Nom tai heo',NULL,1),
    (70,N'Goi bo bop thau',2,95000,N'Goi bo bop thau',NULL,1),
    (71,N'Ga chien gion',3,129000,N'Ga chien gion',NULL,1),
    (72,N'Ga sot bo toi',3,159000,N'Ga sot bo toi',NULL,1),
    (73,N'Ga nuong muoi ot',3,165000,N'Ga nuong muoi ot',NULL,1),
    (74,N'Uc ga ap chao',3,139000,N'Uc ga ap chao',NULL,1),
    (75,N'Ga nau lagu',3,149000,N'Ga nau lagu',NULL,1),
    (76,N'Bo xao hanh tay',4,149000,N'Bo xao hanh tay',NULL,1),
    (77,N'Bo ham tieu xanh',4,169000,N'Bo ham tieu xanh',NULL,1),
    (78,N'Bo cuon nam kim cham',4,179000,N'Bo cuon nam kim cham',NULL,1),
    (79,N'Bo nuong sot BBQ',4,189000,N'Bo nuong sot BBQ',NULL,1),
    (80,N'Bo xao can toi',4,145000,N'Bo xao can toi',NULL,1),
    (81,N'Cha lua chien',5,79000,N'Cha lua chien',NULL,1),
    (82,N'Suon non ram man',5,149000,N'Suon non ram man',NULL,1),
    (83,N'Ba roi chien mam',5,139000,N'Ba roi chien mam',NULL,1),
    (84,N'Nem nuong',5,99000,N'Nem nuong',NULL,1),
    (85,N'Dui heo ham',5,159000,N'Dui heo ham',NULL,1),
    (86,N'Ca loc hap bau',6,189000,N'Ca loc hap bau',NULL,1),
    (87,N'Ca basa kho to',6,119000,N'Ca basa kho to',NULL,1),
    (88,N'Ngheu hap sa',6,99000,N'Ngheu hap sa',NULL,1),
    (89,N'Vem nuong mo hanh',6,129000,N'Vem nuong mo hanh',NULL,1),
    (90,N'Bach tuoc nuong',6,169000,N'Bach tuoc nuong',NULL,1),
    (91,N'Bong cai xao toi',7,65000,N'Bong cai xao toi',NULL,1),
    (92,N'Ca tim nuong mo hanh',7,69000,N'Ca tim nuong mo hanh',NULL,1),
    (93,N'Dau que xao toi',7,59000,N'Dau que xao toi',NULL,1),
    (94,N'Kho qua xao trung',7,59000,N'Kho qua xao trung',NULL,1),
    (95,N'Rau thap cam xao',7,69000,N'Rau thap cam xao',NULL,1),
    (96,N'Com chien trung',8,59000,N'Com chien trung',NULL,1),
    (97,N'Com chien bo',8,89000,N'Com chien bo',NULL,1),
    (98,N'Com ga xoi mo',8,89000,N'Com ga xoi mo',NULL,1),
    (99,N'Com ca kho to',8,85000,N'Com ca kho to',NULL,1),
    (100,N'Com bo luc lac',8,99000,N'Com bo luc lac',NULL,1);
    SET IDENTITY_INSERT dbo.MenuItems OFF;

    /* 5. DINING TABLES */
    SET IDENTITY_INSERT dbo.DiningTables ON;
    INSERT INTO dbo.DiningTables (TableID, TableName, QRCode, Capacity, TableStatus)
    VALUES
    (1,N'Ban 01','QR-TABLE-01',2,N'Trong'),(2,N'Ban 02','QR-TABLE-02',2,N'Trong'),
    (3,N'Ban 03','QR-TABLE-03',2,N'Trong'),(4,N'Ban 04','QR-TABLE-04',2,N'Trong'),
    (5,N'Ban 05','QR-TABLE-05',4,N'Trong'),(6,N'Ban 06','QR-TABLE-06',4,N'Trong'),
    (7,N'Ban 07','QR-TABLE-07',4,N'Trong'),(8,N'Ban 08','QR-TABLE-08',4,N'Trong'),
    (9,N'Ban 09','QR-TABLE-09',4,N'Trong'),(10,N'Ban 10','QR-TABLE-10',4,N'Trong'),
    (11,N'Ban 11','QR-TABLE-11',4,N'Trong'),(12,N'Ban 12','QR-TABLE-12',4,N'Trong'),
    (13,N'Ban 13','QR-TABLE-13',4,N'Trong'),(14,N'Ban 14','QR-TABLE-14',4,N'Trong'),
    (15,N'Ban 15','QR-TABLE-15',4,N'Trong'),(16,N'Ban 16','QR-TABLE-16',6,N'Trong'),
    (17,N'Ban 17','QR-TABLE-17',6,N'Trong'),(18,N'Ban 18','QR-TABLE-18',6,N'Trong'),
    (19,N'Ban 19','QR-TABLE-19',6,N'Trong'),(20,N'Ban 20','QR-TABLE-20',6,N'Trong'),
    (21,N'Ban 21','QR-TABLE-21',8,N'Trong'),(22,N'Ban 22','QR-TABLE-22',8,N'Trong'),
    (23,N'Ban 23','QR-TABLE-23',8,N'Trong'),(24,N'Ban 24','QR-TABLE-24',8,N'Trong'),
    (25,N'Ban 25','QR-TABLE-25',10,N'Trong'),(26,N'Ban 26','QR-TABLE-26',10,N'Trong'),
    (27,N'Ban 27','QR-TABLE-27',10,N'Trong'),(28,N'Ban 28','QR-TABLE-28',4,N'Trong'),
    (29,N'Ban 29','QR-TABLE-29',6,N'Trong'),(30,N'Ban 30','QR-TABLE-30',8,N'Trong');
    SET IDENTITY_INSERT dbo.DiningTables OFF;

    /* 6. ORDERS - 300 don trong 90 ngay gan nhat */
    SET IDENTITY_INSERT dbo.Orders ON;
    DECLARE @i int = 1, @orderTime datetime2(7), @orderStatus nvarchar(20),
            @tableID int, @employeeID int, @orderCode varchar(30);
    WHILE @i <= 300
    BEGIN
        SET @orderTime = DATEADD(minute, -((@i * 37) % 720),
                         DATEADD(day, -((@i - 1) % 90), CONVERT(datetime2(0), GETDATE())));
        SET @orderStatus =
            CASE WHEN @i <= 250 THEN N'Da thanh toan'
                 WHEN @i <= 295 THEN N'Hoan thanh'
                 ELSE N'Da huy' END;
        SET @tableID = ((@i - 1) % 30) + 1;
        SET @employeeID = ((@i - 1) % 5) + 6;
        SET @orderCode = 'DH' + RIGHT('0000' + CAST(@i AS varchar(4)), 4);

        INSERT INTO dbo.Orders
            (OrderID, OrderCode, TableID, EmployeeID, OrderTime, OrderStatus, Note)
        VALUES
            (@i, @orderCode, @tableID, @employeeID, @orderTime, @orderStatus,
             CASE WHEN @i % 9 = 0 THEN N'Khach yeu cau it cay'
                  WHEN @i % 13 = 0 THEN N'Khach co tre em'
                  ELSE NULL END);
        SET @i += 1;
    END;
    SET IDENTITY_INSERT dbo.Orders OFF;

    /* 7. ORDER DETAILS - 3 mon moi don, tong 900 dong */
    SET IDENTITY_INSERT dbo.OrderDetails ON;
    DECLARE @detailID int = 1, @orderID int = 1, @j int,
            @itemStatus nvarchar(20), @orderedAt datetime2(7),
            @startedAt datetime2(7), @completedAt datetime2(7), @servedAt datetime2(7),
            @menuID int, @quantity int, @price decimal(18,2), @statusForOrder nvarchar(20);
    WHILE @orderID <= 300
    BEGIN
        SELECT @orderedAt = OrderTime, @statusForOrder = OrderStatus
        FROM dbo.Orders WHERE OrderID = @orderID;

        SET @j = 1;
        WHILE @j <= 3
        BEGIN
            SET @menuID = ((@orderID * 7 + @j * 13 - 1) % 100) + 1;
            SET @quantity = ((@orderID + @j) % 3) + 1;
            SELECT @price = Price FROM dbo.MenuItems WHERE MenuItemID = @menuID;

            SET @itemStatus =
                CASE WHEN @statusForOrder IN (N'Da thanh toan',N'Hoan thanh') THEN N'Da phuc vu'
                     WHEN @statusForOrder = N'Dang xu ly' THEN
                          CASE WHEN @j = 1 THEN N'San sang' ELSE N'Dang che bien' END
                     WHEN @statusForOrder = N'Moi' THEN N'Moi'
                     ELSE N'Da huy' END;

            SET @startedAt = NULL;
            SET @completedAt = NULL;
            SET @servedAt = NULL;
            IF @itemStatus IN (N'Dang che bien',N'San sang',N'Da phuc vu')
                SET @startedAt = DATEADD(minute, 5, @orderedAt);
            IF @itemStatus IN (N'San sang',N'Da phuc vu')
                SET @completedAt = DATEADD(minute, 20, @orderedAt);
            IF @itemStatus = N'Da phuc vu'
                SET @servedAt = DATEADD(minute, 25, @orderedAt);

            INSERT INTO dbo.OrderDetails
                (OrderDetailID, OrderID, MenuItemID, Quantity, UnitPrice, ItemStatus,
                 OrderedAt, CookingStartedAt, CookingCompletedAt, ServedAt, Note)
            VALUES
                (@detailID, @orderID, @menuID, @quantity, @price, @itemStatus,
                 @orderedAt, @startedAt, @completedAt, @servedAt,
                 CASE WHEN @j = 3 AND @orderID % 10 = 0 THEN N'Khong hanh' ELSE NULL END);

            SET @detailID += 1;
            SET @j += 1;
        END;
        SET @orderID += 1;
    END;
    SET IDENTITY_INSERT dbo.OrderDetails OFF;

    /* 8. PAYMENTS - 250 don da thanh toan */
    DECLARE @paymentID int = 1, @amount decimal(18,2), @method nvarchar(30);
    WHILE @paymentID <= 250
    BEGIN
        SELECT @amount = SUM(Quantity * UnitPrice)
        FROM dbo.OrderDetails WHERE OrderID = @paymentID;

        SET @method = CASE WHEN @paymentID % 2 = 0 THEN N'Chuyen khoan'
                           ELSE N'Tien mat' END;

        INSERT INTO dbo.Payments
            (OrderID, Amount, PaymentMethod, PaymentStatus, PaymentTime, EmployeeID, TransactionCode)
        SELECT @paymentID, @amount, @method, N'Thanh cong',
               DATEADD(minute, 30, OrderTime),
               ((@paymentID - 1) % 3) + 3,
               CASE WHEN @method = N'Tien mat' THEN NULL
                    ELSE 'TX' + RIGHT('000000' + CAST(@paymentID AS varchar(6)), 6) END
        FROM dbo.Orders WHERE OrderID = @paymentID;

        SET @paymentID += 1;
    END;

    /* 9. ORDER STATUS HISTORY */
    DECLARE @historyOrderID int = 1, @historyTime datetime2(7), @historyStatus nvarchar(20);
    WHILE @historyOrderID <= 300
    BEGIN
        SELECT @historyTime = OrderTime, @historyStatus = OrderStatus
        FROM dbo.Orders WHERE OrderID = @historyOrderID;

        INSERT INTO dbo.OrderStatusHistory
            (OrderID, OldStatus, NewStatus, ChangedAt, EmployeeID, Note)
        VALUES
            (@historyOrderID, NULL, N'Moi', @historyTime, ((@historyOrderID - 1) % 5) + 6, N'Tao don hang');

        IF @historyStatus <> N'Moi'
        BEGIN
            INSERT INTO dbo.OrderStatusHistory
                (OrderID, OldStatus, NewStatus, ChangedAt, EmployeeID, Note)
            VALUES
                (@historyOrderID, N'Moi',
                 CASE WHEN @historyStatus = N'Da huy' THEN N'Da huy'
                      ELSE N'Dang xu ly' END,
                 DATEADD(minute, 5, @historyTime), ((@historyOrderID - 1) % 5) + 6,
                 N'Tiep nhan don hang');
        END;

        IF @historyStatus IN (N'Da thanh toan',N'Hoan thanh')
        BEGIN
            INSERT INTO dbo.OrderStatusHistory
                (OrderID, OldStatus, NewStatus, ChangedAt, EmployeeID, Note)
            VALUES
                (@historyOrderID, N'Dang xu ly', N'Hoan thanh',
                 DATEADD(minute, 25, @historyTime), ((@historyOrderID - 1) % 5) + 6,
                 N'Hoan tat phuc vu');
        END;

        IF @historyStatus = N'Da thanh toan'
        BEGIN
            INSERT INTO dbo.OrderStatusHistory
                (OrderID, OldStatus, NewStatus, ChangedAt, EmployeeID, Note)
            VALUES
                (@historyOrderID, N'Hoan thanh', N'Da thanh toan',
                 DATEADD(minute, 30, @historyTime), ((@historyOrderID - 1) % 3) + 3,
                 N'Da thanh toan');
        END;
        SET @historyOrderID += 1;
    END;

    /* 10. ORDER DETAIL STATUS HISTORY */
    DECLARE @historyDetailID int = 1, @detailStatus nvarchar(20), @detailOrderedAt datetime2(7),
            @detailEmployeeID int;
    WHILE @historyDetailID <= 900
    BEGIN
        SELECT @detailStatus = ItemStatus, @detailOrderedAt = OrderedAt
        FROM dbo.OrderDetails WHERE OrderDetailID = @historyDetailID;

        SELECT @detailEmployeeID = EmployeeID
        FROM dbo.Orders
        WHERE OrderID = (SELECT OrderID FROM dbo.OrderDetails WHERE OrderDetailID = @historyDetailID);

        INSERT INTO dbo.OrderDetailStatusHistory
            (OrderDetailID, OldStatus, NewStatus, ChangedAt, EmployeeID, Note)
        VALUES
            (@historyDetailID, NULL, N'Moi', @detailOrderedAt, @detailEmployeeID, N'Tiep nhan mon');

        IF @detailStatus <> N'Moi'
        BEGIN
            INSERT INTO dbo.OrderDetailStatusHistory
                (OrderDetailID, OldStatus, NewStatus, ChangedAt, EmployeeID, Note)
            VALUES
                (@historyDetailID, N'Moi',
                 CASE WHEN @detailStatus = N'Da huy' THEN N'Da huy'
                      ELSE N'Dang che bien' END,
                 DATEADD(minute, 5, @detailOrderedAt), 11, N'Bep tiep nhan mon');
        END;

        IF @detailStatus IN (N'San sang',N'Da phuc vu')
        BEGIN
            INSERT INTO dbo.OrderDetailStatusHistory
                (OrderDetailID, OldStatus, NewStatus, ChangedAt, EmployeeID, Note)
            VALUES
                (@historyDetailID, N'Dang che bien', N'San sang',
                 DATEADD(minute, 20, @detailOrderedAt), 11, N'Mon da che bien xong');
        END;

        IF @detailStatus = N'Da phuc vu'
        BEGIN
            INSERT INTO dbo.OrderDetailStatusHistory
                (OrderDetailID, OldStatus, NewStatus, ChangedAt, EmployeeID, Note)
            VALUES
                (@historyDetailID, N'San sang', N'Da phuc vu',
                 DATEADD(minute, 25, @detailOrderedAt), 6, N'Da phuc vu khach');
        END;

        SET @historyDetailID += 1;
    END;

    COMMIT TRANSACTION;

    PRINT N'Da chen du lieu mau thanh cong.';
    SELECT N'Roles' AS TableName, COUNT(*) AS TotalRows FROM dbo.Roles
    UNION ALL SELECT N'Employees', COUNT(*) FROM dbo.Employees
    UNION ALL SELECT N'Categories', COUNT(*) FROM dbo.Categories
    UNION ALL SELECT N'MenuItems', COUNT(*) FROM dbo.MenuItems
    UNION ALL SELECT N'DiningTables', COUNT(*) FROM dbo.DiningTables
    UNION ALL SELECT N'Orders', COUNT(*) FROM dbo.Orders
    UNION ALL SELECT N'OrderDetails', COUNT(*) FROM dbo.OrderDetails
    UNION ALL SELECT N'Payments', COUNT(*) FROM dbo.Payments
    UNION ALL SELECT N'OrderStatusHistory', COUNT(*) FROM dbo.OrderStatusHistory
    UNION ALL SELECT N'OrderDetailStatusHistory', COUNT(*) FROM dbo.OrderDetailStatusHistory;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
