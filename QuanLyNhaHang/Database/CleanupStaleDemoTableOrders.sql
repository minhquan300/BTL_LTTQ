/*
    Removes only the seeded demo orders currently left on tables 03-25.
    The script validates all 23 expected order codes, completed status,
    absence of payments, and absence of another open order on affected tables.
    Run manually if the same cleanup is needed on another database.
*/
USE [QuanLyNhaHang];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Targets TABLE (OrderID int NOT NULL PRIMARY KEY, TableID int NOT NULL);

    INSERT INTO @Targets (OrderID, TableID)
    SELECT o.OrderID, o.TableID
    FROM dbo.Orders AS o WITH (UPDLOCK, HOLDLOCK)
    WHERE o.OrderID BETWEEN 273 AND 295
      AND o.OrderCode = 'DH' + RIGHT('0000' + CONVERT(varchar(4), o.OrderID), 4)
      AND o.OrderStatus = N'Hoan thanh';

    IF (SELECT COUNT(*) FROM @Targets) <> 23
        THROW 50001, N'Du lieu khong con khop voi 23 don mau du kien; khong xoa gi.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM @Targets AS t
        INNER JOIN dbo.Payments AS p ON p.OrderID = t.OrderID
    )
        THROW 50002, N'Phat hien thanh toan gan voi don mau; khong xoa gi.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM @Targets AS t
        INNER JOIN dbo.Orders AS otherOrder WITH (UPDLOCK, HOLDLOCK)
            ON otherOrder.TableID = t.TableID
           AND otherOrder.OrderID <> t.OrderID
           AND otherOrder.OrderStatus NOT IN (N'Hoan thanh', N'Da thanh toan', N'Da huy')
    )
        THROW 50003, N'Co don dang mo tren mot trong cac ban; khong xoa gi.', 1;

    DELETE h
    FROM dbo.OrderDetailStatusHistory AS h
    INNER JOIN dbo.OrderDetails AS od ON od.OrderDetailID = h.OrderDetailID
    INNER JOIN @Targets AS t ON t.OrderID = od.OrderID;

    DELETE h
    FROM dbo.OrderStatusHistory AS h
    INNER JOIN @Targets AS t ON t.OrderID = h.OrderID;

    DELETE od
    FROM dbo.OrderDetails AS od
    INNER JOIN @Targets AS t ON t.OrderID = od.OrderID;

    DELETE o
    FROM dbo.Orders AS o
    INNER JOIN @Targets AS t ON t.OrderID = o.OrderID;

    UPDATE dt
    SET TableStatus = N'Trong'
    FROM dbo.DiningTables AS dt
    WHERE EXISTS (SELECT 1 FROM @Targets AS t WHERE t.TableID = dt.TableID)
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.Orders AS activeOrder
          WHERE activeOrder.TableID = dt.TableID
            AND activeOrder.OrderStatus NOT IN (N'Hoan thanh', N'Da thanh toan', N'Da huy')
      );

    COMMIT TRANSACTION;
    SELECT N'Da xoa 23 don mau cu va chi tiet lien quan; cac ban duoc dat ve Trong.' AS Result;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
