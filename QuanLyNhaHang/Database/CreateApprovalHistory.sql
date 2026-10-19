/*
    Creates the manager approval audit table.
    Review and execute this script manually against the application database.
    It is intentionally not run by the application or this change.
*/
IF OBJECT_ID(N'dbo.LichSuDuyet', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LichSuDuyet
    (
        MaDuyet BIGINT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_LichSuDuyet PRIMARY KEY,
        MaQuanLyDuyet INT NOT NULL,
        MaNhanVienYeuCau INT NULL,
        TenDangNhapYeuCau NVARCHAR(100) NULL,
        TenNguoiYeuCau NVARCHAR(150) NULL,
        MoTaThaoTac NVARCHAR(500) NOT NULL,
        ThoiGianDuyet DATETIME2(0) NOT NULL
            CONSTRAINT DF_LichSuDuyet_ThoiGianDuyet DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_LichSuDuyet_QuanLy
            FOREIGN KEY (MaQuanLyDuyet) REFERENCES dbo.Employees(EmployeeID),
        CONSTRAINT FK_LichSuDuyet_NguoiYeuCau
            FOREIGN KEY (MaNhanVienYeuCau) REFERENCES dbo.Employees(EmployeeID)
    );

    CREATE INDEX IX_LichSuDuyet_ThoiGianDuyet
        ON dbo.LichSuDuyet(ThoiGianDuyet DESC);
    CREATE INDEX IX_LichSuDuyet_MaQuanLyDuyet_ThoiGianDuyet
        ON dbo.LichSuDuyet(MaQuanLyDuyet, ThoiGianDuyet DESC);
END;
GO
