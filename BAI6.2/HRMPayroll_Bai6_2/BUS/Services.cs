using System.Data;
using HRMPayroll.DAL;
using HRMPayroll.DTO;
namespace HRMPayroll.BUS;
public class EmployeeService
{
 EmployeeRepository repo=new();public DataTable GetAll(string key="")=>repo.GetAll(key);
 public void Save(Employee e,bool insert){if(string.IsNullOrWhiteSpace(e.HoTen)||e.MaPB<=0||e.LuongCoBan<=0||e.HeSoLuong<=0)throw new ArgumentException("Kiểm tra họ tên, phòng ban, lương cơ bản và hệ số lương.");repo.Save(e,insert);}
 public void Delete(int id)=>repo.Delete(id);
}
public class DepartmentService{public List<Department> GetAll()=>new DepartmentRepository().GetAll();}
public class AttendanceService
{
 AttendanceRepository repo=new();
 public DataTable Load(string month)=>repo.LoadMonth(month);
 public void Save(DataTable table,string month)
 {
  foreach(DataRow r in table.Rows)if(r.RowState!=DataRowState.Deleted)
  {double days=Convert.ToDouble(r["SoNgayCong"]);double ot=Convert.ToDouble(r["SoGioOT"]);if(days<0||days>31||ot<0)throw new ArgumentException("Ngày công phải từ 0 đến 31, giờ OT không được âm.");}
  repo.SaveMonth(table,month);
 }
}
public class PayrollService
{
 PayrollRepository repo=new();public void Close(string month)=>repo.CloseMonth(month);public DataTable Get(string month)=>repo.GetPayroll(month);
}
