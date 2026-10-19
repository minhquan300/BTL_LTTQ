-- Chạy thủ công một lần trên database QuanLyNhaHang để bật nhật ký in hóa đơn.
-- Script này được cung cấp riêng và không được ứng dụng tự chạy.
IF OBJECT_ID(N'dbo.InvoicePrintLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InvoicePrintLog
    (
        InvoicePrintLogID BIGINT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_InvoicePrintLog PRIMARY KEY,
        OrderID INT NOT NULL,
        EmployeeID INT NULL,
        PrintedAt DATETIME2(7) NOT NULL
            CONSTRAINT DF_InvoicePrintLog_PrintedAt DEFAULT (SYSDATETIME()),
        IsReprint BIT NOT NULL,
        PrintSucceeded BIT NOT NULL,
        ErrorMessage NVARCHAR(500) NULL,
        CONSTRAINT FK_InvoicePrintLog_Orders FOREIGN KEY (OrderID)
            REFERENCES dbo.Orders(OrderID),
        CONSTRAINT FK_InvoicePrintLog_Employees FOREIGN KEY (EmployeeID)
            REFERENCES dbo.Employees(EmployeeID)
    );

    CREATE INDEX IX_InvoicePrintLog_OrderID_PrintedAt
        ON dbo.InvoicePrintLog(OrderID, PrintedAt DESC)
        INCLUDE (IsReprint, PrintSucceeded);
END;
