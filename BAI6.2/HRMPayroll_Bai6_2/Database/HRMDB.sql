/* Chạy trong SQL Server Management Studio. Script tạo lại bảng, hãy sao lưu nếu có dữ liệu cũ. */
IF DB_ID(N'HRMDB') IS NULL CREATE DATABASE HRMDB;
GO
USE HRMDB;
GO
IF OBJECT_ID('dbo.BangLuong','U') IS NOT NULL DROP TABLE dbo.BangLuong;
IF OBJECT_ID('dbo.BangChamCong','U') IS NOT NULL DROP TABLE dbo.BangChamCong;
IF OBJECT_ID('dbo.NhanVien','U') IS NOT NULL DROP TABLE dbo.NhanVien;
IF OBJECT_ID('dbo.PhongBan','U') IS NOT NULL DROP TABLE dbo.PhongBan;
GO
CREATE TABLE PhongBan(
 MaPB INT IDENTITY(1,1) PRIMARY KEY,
 TenPB NVARCHAR(100) NOT NULL,
 Mota NVARCHAR(255) NULL
);
CREATE TABLE NhanVien(
 MaNV INT IDENTITY(1,1) PRIMARY KEY,
 HoTen NVARCHAR(100) NOT NULL,
 NgaySinh DATETIME NULL,
 CCCD VARCHAR(20) NULL UNIQUE,
 MaPB INT NOT NULL,
 LuongCoBan DECIMAL(18,2) NOT NULL CHECK(LuongCoBan>0),
 HeSoLuong FLOAT NOT NULL CONSTRAINT DF_NhanVien_HeSo DEFAULT 1.0,
 CONSTRAINT FK_NhanVien_PhongBan FOREIGN KEY(MaPB) REFERENCES PhongBan(MaPB)
);
CREATE TABLE BangChamCong(
 MaCong INT IDENTITY(1,1) PRIMARY KEY,
 MaNV INT NOT NULL,
 ThangNam VARCHAR(7) NOT NULL,
 SoNgayCong FLOAT NOT NULL CHECK(SoNgayCong>=0 AND SoNgayCong<=31),
 SoGioOT FLOAT NOT NULL CONSTRAINT DF_BangChamCong_OT DEFAULT 0 CHECK(SoGioOT>=0),
 DaChot BIT NOT NULL CONSTRAINT DF_BangChamCong_DaChot DEFAULT 0,
 CONSTRAINT FK_BangChamCong_NhanVien FOREIGN KEY(MaNV) REFERENCES NhanVien(MaNV),
 CONSTRAINT UQ_BangChamCong_NhanVien_Thang UNIQUE(MaNV,ThangNam)
);
CREATE TABLE BangLuong(
 MaLuong INT IDENTITY(1,1) NOT NULL,
 MaNV INT NOT NULL,
 ThangNam VARCHAR(7) NOT NULL,
 ThucLinh DECIMAL(18,2) NOT NULL,
 CONSTRAINT PK_BangLuong PRIMARY KEY(MaNV,ThangNam),
 CONSTRAINT UQ_BangLuong_MaLuong UNIQUE(MaLuong),
 CONSTRAINT FK_BangLuong_NhanVien FOREIGN KEY(MaNV) REFERENCES NhanVien(MaNV)
);
GO
/* Trigger ngăn sửa/xóa chấm công sau khi đã chốt lương */
CREATE TRIGGER TR_BangChamCong_KhoaSauChot ON BangChamCong
AFTER INSERT,UPDATE,DELETE
AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM deleted d JOIN BangChamCong bc ON bc.MaCong=d.MaCong WHERE bc.DaChot=1)
 BEGIN
  RAISERROR(N'Bảng chấm công tháng này đã chốt lương, không được chỉnh sửa.',16,1);
  ROLLBACK TRANSACTION; RETURN;
 END
END;
GO
INSERT INTO PhongBan(TenPB,Mota) VALUES
(N'Ban Giám đốc',N'Quản lý và điều hành'),(N'Kế toán',N'Tài chính và kế toán'),
(N'Kinh doanh',N'Kinh doanh và bán hàng'),(N'Kỹ thuật',N'Công nghệ và kỹ thuật');
INSERT INTO NhanVien(HoTen,NgaySinh,CCCD,MaPB,LuongCoBan,HeSoLuong) VALUES
(N'Nguyễn Văn An','2000-05-12','001200000001',2,9000000,1.5),
(N'Trần Thị Bình','1999-08-20','001199000002',3,8500000,1.3),
(N'Lê Minh Cường','2001-02-14','001201000003',4,10000000,1.7);
GO
