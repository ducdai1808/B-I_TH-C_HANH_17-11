namespace HRMPayroll.DTO;
public class Employee
{
 public int MaNV{get;set;} public string HoTen{get;set;}="";
 public DateTime? NgaySinh{get;set;} public string? CCCD{get;set;}
 public int MaPB{get;set;} public decimal LuongCoBan{get;set;} public double HeSoLuong{get;set;}=1;
}
public class Department
{
 public int MaPB{get;set;} public string TenPB{get;set;}="";
 public string? Mota{get;set;} public override string ToString()=>TenPB;
}
