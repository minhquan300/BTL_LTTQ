/*
   Close only the pending demo orders created by SeedData.sql (OrderID 271-295).
   This leaves genuine restaurant orders untouched.
*/
USE [QuanLyNhaHang];
GO
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE od
    SET ItemStatus = N'Da phuc vu',
        CookingStartedAt = COALESCE(CookingStartedAt, DATEADD(minute, 5, OrderedAt)),
        CookingCompletedAt = COALESCE(CookingCompletedAt, DATEADD(minute, 20, OrderedAt)),
        ServedAt = COALESCE(ServedAt, DATEADD(minute, 25, OrderedAt))
    FROM dbo.OrderDetails AS od
    INNER JOIN dbo.Orders AS o ON o.OrderID = od.OrderID
    WHERE o.OrderID BETWEEN 271 AND 295
      AND o.OrderCode = 'DH' + RIGHT('0000' + CONVERT(varchar(4), o.OrderID), 4)
      AND o.OrderStatus IN (N'Moi', N'Dang xu ly');

    UPDATE o
    SET OrderStatus = N'Hoan thanh'
    FROM dbo.Orders AS o
    WHERE o.OrderID BETWEEN 271 AND 295
      AND o.OrderCode = 'DH' + RIGHT('0000' + CONVERT(varchar(4), o.OrderID), 4)
      AND o.OrderStatus IN (N'Moi', N'Dang xu ly');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
