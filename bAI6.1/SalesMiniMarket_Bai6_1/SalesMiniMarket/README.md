# RETAIL SALES SYSTEM - Bài 6.1

## Công nghệ
- C# Windows Forms, .NET 10
- SQL Server
- Microsoft.Data.SqlClient
- Kiến trúc: DTO - DAL - BUS - GUI

## 1. Tạo cơ sở dữ liệu
1. Mở SQL Server Management Studio (SSMS).
2. Mở file `Database/SalesDB.sql`.
3. Chạy toàn bộ script bằng Execute (F5).
4. Script tạo CSDL `SalesDB`, 5 bảng và dữ liệu mẫu.

> Lưu ý: Script có lệnh DROP TABLE để tạo lại cấu trúc từ đầu. Nếu đã có dữ liệu cần giữ, hãy sao lưu trước khi chạy.

## 2. Cấu hình kết nối
Mở `App.config`, sửa `Server=.` thành tên SQL Server của máy bạn.
Ví dụ SQL Express: `Server=.\\SQLEXPRESS;Database=SalesDB;Trusted_Connection=True;TrustServerCertificate=True;`

## 3. Mở và chạy
1. Mở `SalesMiniMarket.csproj` bằng Visual Studio 2022 bản hỗ trợ .NET 10.
2. Restore NuGet packages.
3. Nhấn F5.

## 4. Chức năng
- Sản phẩm: thêm, sửa, xóa, xem, tìm theo mã/tên với LIKE và SqlParameter.
- Khách hàng: thêm, sửa, xóa, xem.
- Nhân viên: thêm, sửa, xóa, xem.
- Bán hàng: chọn nhân viên/khách lẻ hoặc khách hàng, thêm sản phẩm vào giỏ, thanh toán.
- Thanh toán sử dụng một SqlTransaction: tạo hóa đơn, cập nhật tồn kho có điều kiện, thêm chi tiết hóa đơn, cộng điểm; lỗi bất kỳ sẽ Rollback.
- Báo cáo: doanh thu ngày/tháng bằng ExecuteScalar; Top 5 bằng SqlDataReader.

## 5. Quy tắc điểm
Điểm được cộng bằng phần nguyên của `TongTien / 100000` (ví dụ 250.000đ = 2 điểm).

## 6. Cấu trúc
- DTO: các lớp dữ liệu
- DAL: kết nối và truy vấn SQL
- BUS: kiểm tra dữ liệu/nghiệp vụ
- GUI: giao diện WinForms

## Ghi chú
Đây là bộ khung thực hành có giao diện tạo bằng code, không dùng Designer. Hãy chạy SQL script trước khi mở ứng dụng. Nếu SQL Server dùng xác thực SQL Login, đổi connection string sang `User Id=...;Password=...;` và không chia sẻ mật khẩu.
