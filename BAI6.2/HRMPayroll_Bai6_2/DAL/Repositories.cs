using Microsoft.Data.SqlClient;
using System.Data;
using HRMPayroll.DTO;
namespace HRMPayroll.DAL;
public class EmployeeRepository
{
 public DataTable GetAll(string key="")
 {
  using var c=Db.Open(); using var cmd=new SqlCommand(@"SELECT nv.MaNV,nv.HoTen,nv.NgaySinh,nv.CCCD,nv.MaPB,pb.TenPB,nv.LuongCoBan,nv.HeSoLuong
  FROM NhanVien nv JOIN PhongBan pb ON nv.MaPB=pb.MaPB
  WHERE nv.HoTen LIKE @kw OR nv.CCCD LIKE @kw OR pb.TenPB LIKE @kw ORDER BY nv.MaNV DESC",c);
  cmd.Parameters.Add("@kw",SqlDbType.NVarChar,150).Value="%"+key+"%";
  using var r=cmd.ExecuteReader();var t=new DataTable();t.Load(r);return t;
 }
 public void Save(Employee e,bool insert)
 {
  using var c=Db.Open();using var cmd=new SqlCommand(insert?
  "INSERT INTO NhanVien(HoTen,NgaySinh,CCCD,MaPB,LuongCoBan,HeSoLuong) VALUES(@ten,@ns,@cccd,@pb,@luong,@hs)":
  "UPDATE NhanVien SET HoTen=@ten,NgaySinh=@ns,CCCD=@cccd,MaPB=@pb,LuongCoBan=@luong,HeSoLuong=@hs WHERE MaNV=@ma",c);
  cmd.Parameters.Add("@ten",SqlDbType.NVarChar,100).Value=e.HoTen;
  cmd.Parameters.Add("@ns",SqlDbType.DateTime).Value=(object?)e.NgaySinh??DBNull.Value;
  cmd.Parameters.Add("@cccd",SqlDbType.VarChar,20).Value=(object?)e.CCCD??DBNull.Value;
  cmd.Parameters.Add("@pb",SqlDbType.Int).Value=e.MaPB;
  var p=cmd.Parameters.Add("@luong",SqlDbType.Decimal);p.Precision=18;p.Scale=2;p.Value=e.LuongCoBan;
  cmd.Parameters.Add("@hs",SqlDbType.Float).Value=e.HeSoLuong;
  if(!insert)cmd.Parameters.Add("@ma",SqlDbType.Int).Value=e.MaNV;
  cmd.ExecuteNonQuery();
 }
 public void Delete(int id){using var c=Db.Open();using var cmd=new SqlCommand("DELETE FROM NhanVien WHERE MaNV=@id",c);cmd.Parameters.Add("@id",SqlDbType.Int).Value=id;cmd.ExecuteNonQuery();}
}
public class DepartmentRepository
{
 public List<Department> GetAll(){using var c=Db.Open();using var cmd=new SqlCommand("SELECT MaPB,TenPB,Mota FROM PhongBan ORDER BY TenPB",c);using var r=cmd.ExecuteReader();var list=new List<Department>();while(r.Read())list.Add(new(){MaPB=r.GetInt32(0),TenPB=r.GetString(1),Mota=r.IsDBNull(2)?null:r.GetString(2)});return list;}
}
public class AttendanceRepository
{
 // Disconnected model: DataAdapter + DataTable; schema includes a primary key for CommandBuilder.
 public DataTable LoadMonth(string month)
 {
  using var c=Db.Open();
  using var da=new SqlDataAdapter(@"SELECT MaCong,MaNV,ThangNam,SoNgayCong,SoGioOT FROM BangChamCong WHERE ThangNam=@thang",c);
  da.SelectCommand!.Parameters.Add("@thang",SqlDbType.VarChar,7).Value=month;
  da.MissingSchemaAction=MissingSchemaAction.AddWithKey;
  var table=new DataTable();da.Fill(table);
  // Add employees not yet having attendance rows; new rows get MaCong=DBNull and are inserted on save.
  using var emp=new SqlCommand(@"SELECT nv.MaNV,nv.HoTen FROM NhanVien nv WHERE NOT EXISTS
    (SELECT 1 FROM BangChamCong bc WHERE bc.MaNV=nv.MaNV AND bc.ThangNam=@thang)",c);
  emp.Parameters.Add("@thang",SqlDbType.VarChar,7).Value=month;
  using var reader=emp.ExecuteReader();
  table.Columns.Add("HoTen",typeof(string));
  while(reader.Read()){var row=table.NewRow();row["MaCong"]=DBNull.Value;row["MaNV"]=reader.GetInt32(0);row["ThangNam"]=month;row["SoNgayCong"]=0d;row["SoGioOT"]=0d;row["HoTen"]=reader.GetString(1);table.Rows.Add(row);}
  if(!table.Columns.Contains("HoTen"))table.Columns.Add("HoTen",typeof(string));
  // Fill employee names for existing attendance records.
  foreach(DataRow row in table.Rows) if(row["HoTen"]==DBNull.Value || string.IsNullOrEmpty(row["HoTen"].ToString()))
  {
   using var nameCmd=new SqlCommand("SELECT HoTen FROM NhanVien WHERE MaNV=@id",c);
   nameCmd.Parameters.Add("@id",SqlDbType.Int).Value=row["MaNV"];row["HoTen"]=nameCmd.ExecuteScalar()?.ToString()??"";
  }
  return table;
 }
 public void SaveMonth(DataTable table,string month)
 {
  using var c=Db.Open();
  // Adapter's SELECT must target only the updatable table; DataTable may carry a display-only HoTen column.
  using var da=new SqlDataAdapter("SELECT MaCong,MaNV,ThangNam,SoNgayCong,SoGioOT FROM BangChamCong WHERE ThangNam=@thang",c);
  da.SelectCommand!.Parameters.Add("@thang",SqlDbType.VarChar,7).Value=month;
  da.MissingSchemaAction=MissingSchemaAction.AddWithKey;
  using var schema=new DataTable();da.FillSchema(schema,SchemaType.Source);
  // Copy only persisted columns and preserve row states.
  var changes=table.GetChanges();if(changes==null)return;
  var work=schema.Clone();
  foreach(DataRow source in changes.Rows)
  {
   var target=work.NewRow();
   foreach(string col in new[]{"MaCong","MaNV","ThangNam","SoNgayCong","SoGioOT"})
     target[col]=source[col] is DBNull?DBNull.Value:source[col];
   work.Rows.Add(target);
   if(source.RowState==DataRowState.Deleted)target.Delete();
  }
  using var builder=new SqlCommandBuilder(da);
  da.InsertCommand=builder.GetInsertCommand();da.UpdateCommand=builder.GetUpdateCommand();da.DeleteCommand=builder.GetDeleteCommand();
  da.Update(work);
  table.AcceptChanges();
 }
}
public class PayrollRepository
{
 public void CloseMonth(string month)
 {
  using var c=Db.Open();using var tx=c.BeginTransaction();
  try
  {
   using(var check=new SqlCommand("SELECT COUNT(*) FROM BangLuong WHERE ThangNam=@t",c,tx))
   {check.Parameters.Add("@t",SqlDbType.VarChar,7).Value=month;if((int)check.ExecuteScalar()!>0)throw new InvalidOperationException("Tháng này đã chốt lương.");}
   using(var insert=new SqlCommand(@"INSERT INTO BangLuong(MaNV,ThangNam,ThucLinh)
   SELECT nv.MaNV,@t,CAST((nv.LuongCoBan*nv.HeSoLuong/22.0)*bc.SoNgayCong+(bc.SoGioOT*50000) AS DECIMAL(18,2))
   FROM NhanVien nv INNER JOIN BangChamCong bc ON nv.MaNV=bc.MaNV
   WHERE bc.ThangNam=@t",c,tx))
   {insert.Parameters.Add("@t",SqlDbType.VarChar,7).Value=month;int count=insert.ExecuteNonQuery();if(count==0)throw new InvalidOperationException("Không có dữ liệu chấm công để chốt lương.");}
   // Lock attendance for this month by marking it read-only at database level.
   using(var lockCmd=new SqlCommand("UPDATE BangChamCong SET DaChot=1 WHERE ThangNam=@t",c,tx))
   {lockCmd.Parameters.Add("@t",SqlDbType.VarChar,7).Value=month;lockCmd.ExecuteNonQuery();}
   tx.Commit();
  }
  catch{tx.Rollback();throw;}
 }
 public DataTable GetPayroll(string month){using var c=Db.Open();using var da=new SqlDataAdapter(@"SELECT bl.MaNV,nv.HoTen,bl.ThangNam,bl.ThucLinh FROM BangLuong bl JOIN NhanVien nv ON bl.MaNV=nv.MaNV WHERE bl.ThangNam=@t ORDER BY nv.HoTen",c);da.SelectCommand!.Parameters.Add("@t",SqlDbType.VarChar,7).Value=month;var t=new DataTable();da.Fill(t);return t;}
}
