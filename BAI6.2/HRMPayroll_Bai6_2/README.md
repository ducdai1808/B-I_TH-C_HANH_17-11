# Bài 6.2 - HRM & Payroll System

## Công nghệ
- C# Windows Forms, .NET 10
- SQL Server + Microsoft.Data.SqlClient
- Kiến trúc DTO - DAL - BUS - GUI

## Chạy chương trình
1. Mở `Database/HRMDB.sql` bằng SSMS và chạy toàn bộ script.
2. Sửa `Server=.` trong `App.config` thành tên SQL Server trên máy.
3. Mở `HRMPayroll.csproj` bằng Visual Studio, Restore NuGet, nhấn F5.

## Chức năng
- Hồ sơ nhân sự: xem, thêm, sửa, xóa; chuyển phòng ban; tìm theo họ tên, CCCD, tên phòng ban bằng tham số SQL.
- Chấm công: chọn tháng, nạp bảng theo tháng, nhập ngày công và OT trực tiếp trên DataGridView. DataTable được nạp theo disconnected model; khi lưu, SqlCommandBuilder sinh lệnh và SqlDataAdapter.Update đồng bộ.
- Tính lương: công thức `(LuongCoBan * HeSoLuong / 22) * SoNgayCong + (SoGioOT * 50000)`.
- Chốt lương: một SqlTransaction ghi lương cho toàn bộ nhân viên có dữ liệu chấm công; cập nhật DaChot và trigger khóa dữ liệu tháng đã chốt.

## Cấu trúc thư mục
- DTO: Employee, Department
- DAL: Db, repositories
- BUS: services và validate
- GUI: MainForm
- Database: HRMDB.sql

## Lưu ý
- Tháng lưu theo định dạng `MM/yyyy`, ví dụ `11/2026`.
- Chấm công chỉ được chốt một lần cho mỗi tháng.
- Trigger khóa chấm công là biện pháp bảo vệ bổ sung tại SQL Server.
- Script có DROP TABLE để tạo mới dữ liệu mẫu; sao lưu trước khi chạy lại.
