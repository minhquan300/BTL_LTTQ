/*
    Authentication security migration for SQL Server.
    Review and run manually against the intended database after taking a backup.
    This script does NOT rewrite existing password values: old SHA-256 / legacy
    values are upgraded by the application after a successful sign-in.
*/
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.LichSuDangNhap', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LichSuDangNhap
    (
        LoginHistoryID BIGINT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_LichSuDangNhap PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL,
        EmployeeID INT NULL,
        LoginTime DATETIME2(0) NOT NULL,
        IsSuccess BIT NOT NULL
    );

    CREATE INDEX IX_LichSuDangNhap_LoginTime
        ON dbo.LichSuDangNhap (LoginTime DESC);

    CREATE INDEX IX_LichSuDangNhap_Username_LoginTime
        ON dbo.LichSuDangNhap (Username, LoginTime DESC);
END;

IF OBJECT_ID(N'dbo.LichSuBaoMat', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LichSuBaoMat
    (
        SecurityLogID BIGINT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_LichSuBaoMat PRIMARY KEY,
        EventType NVARCHAR(60) NOT NULL,
        Username NVARCHAR(100) NULL,
        Details NVARCHAR(250) NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL
    );

    CREATE INDEX IX_LichSuBaoMat_CreatedAt
        ON dbo.LichSuBaoMat (CreatedAt DESC);
END;

/* PasswordHash is already NVARCHAR(255) in QLBH.sql, enough for the PBKDF2 format. */
COMMIT TRANSACTION;
GO
