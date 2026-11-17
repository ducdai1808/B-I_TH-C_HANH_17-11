using HRMPayroll.BUS;
using HRMPayroll.DTO;
using System.Data;
namespace HRMPayroll.GUI;
public class MainForm:Form
{
 EmployeeService employees=new();DepartmentService departments=new();AttendanceService attendance=new();PayrollService payroll=new();
 TabControl tabs=new(){Dock=DockStyle.Fill};
 DataGridView empGrid=Grid(),attGrid=Grid(),payGrid=Grid();
 TextBox name=new(),cccd=new(),salary=new(),factor=new(),search=new();
 DateTimePicker birth=new(){Format=DateTimePickerFormat.Custom,CustomFormat="dd/MM/yyyy"};
 ComboBox department=new(){DropDownStyle=ComboBoxStyle.DropDownList};
 DateTimePicker monthPicker=new(){Format=DateTimePickerFormat.Custom,CustomFormat="MM/yyyy",ShowUpDown=true};
 DataTable? attendanceTable; bool loading=false;
 public MainForm(){Text="HRM & PAYROLL SYSTEM - Quản lý nhân sự và tính lương";Width=1200;Height=760;StartPosition=FormStartPosition.CenterScreen;Controls.Add(tabs);BuildEmployees();BuildAttendance();BuildPayroll();Reload();}
 static DataGridView Grid()=>new(){Dock=DockStyle.Fill,ReadOnly=false,AllowUserToAddRows=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
 Button Btn(string s,Action a){var b=new Button{Text=s,AutoSize=true,Padding=new Padding(8,4,8,4)};b.Click+=(_,_)=>Run(a);return b;}
 void Run(Action a){try{a();}catch(Exception ex){MessageBox.Show(ex.Message,"Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
 FlowLayoutPanel Bar()=>new(){Dock=DockStyle.Top,AutoSize=true,Padding=new Padding(8),WrapContents=true};
 void AddTab(string title,Control c){var t=new TabPage(title);t.Controls.Add(c);tabs.TabPages.Add(t);}
 void BuildEmployees()
 {
  var p=new Panel{Dock=DockStyle.Fill};var b=Bar();
  foreach(var (s,x,w) in new[]{("Họ tên",name,150),("CCCD",cccd,130),("Lương CB",salary,100),("Hệ số",factor,70),("Tìm",search,130)})
  {b.Controls.Add(new Label{Text=s,AutoSize=true,Padding=new Padding(3,8,0,0)});x.Width=w;b.Controls.Add(x);}
  b.Controls.Add(new Label{Text="Ngày sinh",AutoSize=true,Padding=new Padding(3,8,0,0)});birth.Width=110;b.Controls.Add(birth);
  b.Controls.Add(new Label{Text="Phòng ban",AutoSize=true,Padding=new Padding(3,8,0,0)});department.Width=150;b.Controls.Add(department);
  b.Controls.Add(Btn("Tìm kiếm",()=>empGrid.DataSource=employees.GetAll(search.Text)));
  b.Controls.Add(Btn("Thêm",()=>{employees.Save(ReadEmployee(),true);ReloadEmployees();}));
  b.Controls.Add(Btn("Sửa / Chuyển PB",()=>{int id=SelectedId();var e=ReadEmployee();e.MaNV=id;employees.Save(e,false);ReloadEmployees();}));
  b.Controls.Add(Btn("Xóa",()=>{employees.Delete(SelectedId());ReloadEmployees();}));
  empGrid.SelectionChanged+=(_,_)=>{if(empGrid.CurrentRow?.DataBoundItem is DataRowView r){name.Text=r["HoTen"].ToString();cccd.Text=r["CCCD"].ToString();salary.Text=r["LuongCoBan"].ToString();factor.Text=r["HeSoLuong"].ToString();if(DateTime.TryParse(r["NgaySinh"]?.ToString(),out var d))birth.Value=d;if(r["MaPB"]!=DBNull.Value)department.SelectedValue=Convert.ToInt32(r["MaPB"]);}};
  p.Controls.Add(empGrid);p.Controls.Add(b);AddTab("Hồ sơ nhân sự",p);
 }
 Employee ReadEmployee()=>new(){HoTen=name.Text.Trim(),CCCD=string.IsNullOrWhiteSpace(cccd.Text)?null:cccd.Text.Trim(),NgaySinh=birth.Value,MaPB=Convert.ToInt32(department.SelectedValue),LuongCoBan=decimal.Parse(salary.Text),HeSoLuong=double.Parse(factor.Text)};
 int SelectedId()=>Convert.ToInt32(empGrid.CurrentRow?.Cells["MaNV"].Value??throw new InvalidOperationException("Hãy chọn nhân viên."));
 void BuildAttendance()
 {
  var p=new Panel{Dock=DockStyle.Fill};var b=Bar();b.Controls.Add(new Label{Text="Tháng chấm công",AutoSize=true,Padding=new Padding(3,8,0,0)});b.Controls.Add(monthPicker);
  b.Controls.Add(Btn("Tải bảng chấm công",LoadAttendance));b.Controls.Add(Btn("Lưu chấm công (DataAdapter.Update)",SaveAttendance));
  p.Controls.Add(attGrid);p.Controls.Add(b);AddTab("Chấm công",p);
  attGrid.DataError+=(_,e)=>{MessageBox.Show("Dữ liệu nhập không hợp lệ.");e.ThrowException=false;};
 }
 string MonthKey()=>monthPicker.Value.ToString("MM/yyyy");
 void LoadAttendance()
 {
  attendanceTable=attendance.Load(MonthKey());loading=true;attGrid.DataSource=attendanceTable;
  if(attGrid.Columns.Contains("MaCong"))attGrid.Columns["MaCong"].ReadOnly=true;
  if(attGrid.Columns.Contains("MaNV"))attGrid.Columns["MaNV"].ReadOnly=true;
  if(attGrid.Columns.Contains("ThangNam"))attGrid.Columns["ThangNam"].ReadOnly=true;
  if(attGrid.Columns.Contains("HoTen"))attGrid.Columns["HoTen"].ReadOnly=true;
  loading=false;
 }
 void SaveAttendance(){if(attendanceTable==null)throw new InvalidOperationException("Hãy tải bảng chấm công trước.");attGrid.EndEdit();attendance.Save(attendanceTable,MonthKey());MessageBox.Show("Đã lưu chấm công.");LoadAttendance();}
 void BuildPayroll()
 {
  var p=new Panel{Dock=DockStyle.Fill};var b=Bar();b.Controls.Add(new Label{Text="Tháng tính lương",AutoSize=true,Padding=new Padding(3,8,0,0)});var pick=new DateTimePicker{Format=DateTimePickerFormat.Custom,CustomFormat="MM/yyyy",ShowUpDown=true};b.Controls.Add(pick);
  b.Controls.Add(Btn("Chốt lương tháng",()=>{string m=pick.Value.ToString("MM/yyyy");if(MessageBox.Show($"Chốt lương tháng {m}? Sau khi chốt không được sửa chấm công.","Xác nhận",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;payroll.Close(m);payGrid.DataSource=payroll.Get(m);MessageBox.Show("Chốt lương thành công.");}));
  b.Controls.Add(Btn("Xem bảng lương",()=>payGrid.DataSource=payroll.Get(pick.Value.ToString("MM/yyyy"))));
  p.Controls.Add(payGrid);p.Controls.Add(b);AddTab("Tính lương",p);
 }
 void ReloadEmployees()=>empGrid.DataSource=employees.GetAll(search.Text);
 void Reload(){Run(()=>{department.DataSource=departments.GetAll();department.DisplayMember="TenPB";department.ValueMember="MaPB";ReloadEmployees();});}
}
