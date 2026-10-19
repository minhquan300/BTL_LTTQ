/* Run once against an existing restaurant database before using the updated payment dialog. */
IF COL_LENGTH(N'dbo.Orders', N'PaymentMethod') IS NULL
    ALTER TABLE dbo.Orders ADD PaymentMethod nvarchar(30) NULL;
GO

IF COL_LENGTH(N'dbo.Orders', N'CashReceived') IS NULL
    ALTER TABLE dbo.Orders ADD CashReceived decimal(18,2) NOT NULL
        CONSTRAINT DF_Orders_CashReceived DEFAULT (0) WITH VALUES;
GO

IF COL_LENGTH(N'dbo.Orders', N'ChangeAmount') IS NULL
    ALTER TABLE dbo.Orders ADD ChangeAmount decimal(18,2) NOT NULL
        CONSTRAINT DF_Orders_ChangeAmount DEFAULT (0) WITH VALUES;
GO

IF COL_LENGTH(N'dbo.Orders', N'DiscountAmount') IS NULL
    ALTER TABLE dbo.Orders ADD DiscountAmount decimal(18,2) NOT NULL
        CONSTRAINT DF_Orders_DiscountAmount DEFAULT (0) WITH VALUES;
GO

IF COL_LENGTH(N'dbo.Orders', N'DiscountApprovedBy') IS NULL
    ALTER TABLE dbo.Orders ADD DiscountApprovedBy int NULL;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_Orders_DiscountApprovedBy'
      AND parent_object_id = OBJECT_ID(N'dbo.Orders')
)
    ALTER TABLE dbo.Orders WITH CHECK
        ADD CONSTRAINT FK_Orders_DiscountApprovedBy
        FOREIGN KEY (DiscountApprovedBy) REFERENCES dbo.Employees(EmployeeID);
GO

/* Backfill the main method for historical paid orders where a payment exists. */
UPDATE o
SET o.PaymentMethod = p.PaymentMethod
FROM dbo.Orders AS o
OUTER APPLY
(
    SELECT TOP (1) p.PaymentMethod
    FROM dbo.Payments AS p
    WHERE p.OrderID = o.OrderID AND p.PaymentStatus = N'Thanh cong'
    ORDER BY p.PaymentTime DESC, p.PaymentID DESC
) AS p
WHERE o.OrderStatus = N'Da thanh toan'
  AND o.PaymentMethod IS NULL;
GO
