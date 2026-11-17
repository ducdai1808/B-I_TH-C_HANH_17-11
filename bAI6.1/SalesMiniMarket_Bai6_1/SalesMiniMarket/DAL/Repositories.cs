using Microsoft.Data.SqlClient;
using SalesMiniMarket.DTO;
using System.Data;
namespace SalesMiniMarket.DAL;

public class ProductRepository
{
    public DataTable GetAll(string keyword = "")
    {
        using var c = Db.Open();
        using var cmd = new SqlCommand(@"SELECT MaSP,TenSP,GiaBan,SoLuongTon FROM SanPham
            WHERE MaSP LIKE @kw OR TenSP LIKE @kw ORDER BY TenSP", c);
        cmd.Parameters.Add("@kw", SqlDbType.NVarChar, 150).Value = "%" + keyword + "%";
        var t = new DataTable(); using var r = cmd.ExecuteReader(); t.Load(r); return t;
    }
    public void Save(SanPham p, bool insert)
    {
        using var c = Db.Open();
        string sql = insert
            ? "INSERT INTO SanPham(MaSP,TenSP,GiaBan,SoLuongTon) VALUES(@ma,@ten,@gia,@ton)"
            : "UPDATE SanPham SET TenSP=@ten,GiaBan=@gia,SoLuongTon=@ton WHERE MaSP=@ma";
        using var cmd = new SqlCommand(sql,c);
        cmd.Parameters.Add("@ma",SqlDbType.VarChar,20).Value=p.MaSP;
        cmd.Parameters.Add("@ten",SqlDbType.NVarChar,150).Value=p.TenSP;
        cmd.Parameters.Add("@gia",SqlDbType.Decimal).Value=p.GiaBan;
        cmd.Parameters["@gia"].Precision=18; cmd.Parameters["@gia"].Scale=2;
        cmd.Parameters.Add("@ton",SqlDbType.Int).Value=p.SoLuongTon;
        cmd.ExecuteNonQuery();
    }
    public void Delete(string id)
    {
        using var c=Db.Open(); using var cmd=new SqlCommand("DELETE FROM SanPham WHERE MaSP=@ma",c);
        cmd.Parameters.Add("@ma",SqlDbType.VarChar,20).Value=id; cmd.ExecuteNonQuery();
    }
    public SanPham? Find(string id)
    {
        using var c=Db.Open(); using var cmd=new SqlCommand("SELECT MaSP,TenSP,GiaBan,SoLuongTon FROM SanPham WHERE MaSP=@ma",c);
        cmd.Parameters.Add("@ma",SqlDbType.VarChar,20).Value=id;
        using var r=cmd.ExecuteReader();
        return r.Read()?new SanPham{MaSP=r.GetString(0),TenSP=r.GetString(1),GiaBan=r.GetDecimal(2),SoLuongTon=r.GetInt32(3)}:null;
    }
}
public class CustomerRepository
{
    public DataTable GetAll(){using var c=Db.Open(); using var a=new SqlDataAdapter("SELECT MaKH,TenKH,DienThoai,DiemTichLuy FROM KhachHang ORDER BY MaKH DESC",c);var t=new DataTable();a.Fill(t);return t;}
    public void Save(KhachHang x,bool insert)
    {
        using var c=Db.Open(); using var cmd=new SqlCommand(insert?
        "INSERT INTO KhachHang(TenKH,DienThoai,DiemTichLuy) VALUES(@ten,@dt,@diem)":
        "UPDATE KhachHang SET TenKH=@ten,DienThoai=@dt,DiemTichLuy=@diem WHERE MaKH=@ma",c);
        cmd.Parameters.Add("@ten",SqlDbType.NVarChar,100).Value=x.TenKH;
        cmd.Parameters.Add("@dt",SqlDbType.VarChar,15).Value=string.IsNullOrWhiteSpace(x.DienThoai)?DBNull.Value:x.DienThoai;
        cmd.Parameters.Add("@diem",SqlDbType.Int).Value=x.DiemTichLuy;
        if(!insert)cmd.Parameters.Add("@ma",SqlDbType.Int).Value=x.MaKH;
        cmd.ExecuteNonQuery();
    }
    public void Delete(int id){using var c=Db.Open();using var cmd=new SqlCommand("DELETE FROM KhachHang WHERE MaKH=@id",c);cmd.Parameters.Add("@id",SqlDbType.Int).Value=id;cmd.ExecuteNonQuery();}
    public List<KhachHang> GetList(){using var c=Db.Open();using var cmd=new SqlCommand("SELECT MaKH,TenKH,DienThoai,DiemTichLuy FROM KhachHang",c);using var r=cmd.ExecuteReader();var l=new List<KhachHang>();while(r.Read())l.Add(new(){MaKH=r.GetInt32(0),TenKH=r.GetString(1),DienThoai=r.IsDBNull(2)?null:r.GetString(2),DiemTichLuy=r.GetInt32(3)});return l;}
}
public class EmployeeRepository
{
    public DataTable GetAll(){using var c=Db.Open();using var a=new SqlDataAdapter("SELECT MaNV,TenNV,SoDienThoai,ChucVu FROM NhanVien ORDER BY MaNV DESC",c);var t=new DataTable();a.Fill(t);return t;}
    public void Save(NhanVien x,bool insert){using var c=Db.Open();using var cmd=new SqlCommand(insert?
    "INSERT INTO NhanVien(TenNV,SoDienThoai,ChucVu) VALUES(@ten,@dt,@cv)":
    "UPDATE NhanVien SET TenNV=@ten,SoDienThoai=@dt,ChucVu=@cv WHERE MaNV=@ma",c);
    cmd.Parameters.Add("@ten",SqlDbType.NVarChar,100).Value=x.TenNV;
    cmd.Parameters.Add("@dt",SqlDbType.VarChar,15).Value=string.IsNullOrWhiteSpace(x.SoDienThoai)?DBNull.Value:x.SoDienThoai;
    cmd.Parameters.Add("@cv",SqlDbType.NVarChar,50).Value=string.IsNullOrWhiteSpace(x.ChucVu)?DBNull.Value:x.ChucVu;
    if(!insert)cmd.Parameters.Add("@ma",SqlDbType.Int).Value=x.MaNV;cmd.ExecuteNonQuery();}
    public void Delete(int id){using var c=Db.Open();using var cmd=new SqlCommand("DELETE FROM NhanVien WHERE MaNV=@id",c);cmd.Parameters.Add("@id",SqlDbType.Int).Value=id;cmd.ExecuteNonQuery();}
    public List<NhanVien> GetList(){using var c=Db.Open();using var cmd=new SqlCommand("SELECT MaNV,TenNV,SoDienThoai,ChucVu FROM NhanVien",c);using var r=cmd.ExecuteReader();var l=new List<NhanVien>();while(r.Read())l.Add(new(){MaNV=r.GetInt32(0),TenNV=r.GetString(1),SoDienThoai=r.IsDBNull(2)?null:r.GetString(2),ChucVu=r.IsDBNull(3)?null:r.GetString(3)});return l;}
}
public class SalesRepository
{
    public void Checkout(int maNV,int? maKH,List<CartItem> items)
    {
        if(items.Count==0)throw new InvalidOperationException("Giỏ hàng đang trống.");
        using var c=Db.Open(); using var trans=c.BeginTransaction();
        try
        {
            decimal total=items.Sum(x=>x.ThanhTien);
            using(var cmd=new SqlCommand("INSERT INTO HoaDon(MaNV,MaKH,TongTien) OUTPUT INSERTED.MaHD VALUES(@nv,@kh,@tong)",c,trans))
            {
                cmd.Parameters.Add("@nv",SqlDbType.Int).Value=maNV;
                cmd.Parameters.Add("@kh",SqlDbType.Int).Value=(object?)maKH??DBNull.Value;
                var p=cmd.Parameters.Add("@tong",SqlDbType.Decimal);p.Precision=18;p.Scale=2;p.Value=total;
                int maHD=(int)cmd.ExecuteScalar()!;
                foreach(var item in items)
                {
                    using var stock=new SqlCommand("UPDATE SanPham SET SoLuongTon=SoLuongTon-@sl WHERE MaSP=@ma AND SoLuongTon>=@sl",c,trans);
                    stock.Parameters.Add("@sl",SqlDbType.Int).Value=item.SoLuong;
                    stock.Parameters.Add("@ma",SqlDbType.VarChar,20).Value=item.MaSP;
                    if(stock.ExecuteNonQuery()!=1)throw new InvalidOperationException($"Sản phẩm {item.MaSP} không đủ tồn kho.");
                    using var detail=new SqlCommand("INSERT INTO ChiTietHoaDon(MaHD,MaSP,SoLuong,DonGia) VALUES(@hd,@sp,@sl,@gia)",c,trans);
                    detail.Parameters.Add("@hd",SqlDbType.Int).Value=maHD;
                    detail.Parameters.Add("@sp",SqlDbType.VarChar,20).Value=item.MaSP;
                    detail.Parameters.Add("@sl",SqlDbType.Int).Value=item.SoLuong;
                    var price=detail.Parameters.Add("@gia",SqlDbType.Decimal);price.Precision=18;price.Scale=2;price.Value=item.DonGia;
                    detail.ExecuteNonQuery();
                }
                if(maKH.HasValue)
                {
                    using var points=new SqlCommand("UPDATE KhachHang SET DiemTichLuy=DiemTichLuy+@diem WHERE MaKH=@kh",c,trans);
                    points.Parameters.Add("@diem",SqlDbType.Int).Value=(int)(total/100000m);
                    points.Parameters.Add("@kh",SqlDbType.Int).Value=maKH.Value;
                    points.ExecuteNonQuery();
                }
            }
            trans.Commit();
        }
        catch{trans.Rollback();throw;}
    }
    public decimal RevenueToday(){using var c=Db.Open();using var cmd=new SqlCommand("SELECT COALESCE(SUM(TongTien),0) FROM HoaDon WHERE CAST(NgayLap AS date)=CAST(GETDATE() AS date)",c);return Convert.ToDecimal(cmd.ExecuteScalar());}
    public decimal RevenueMonth(){using var c=Db.Open();using var cmd=new SqlCommand("SELECT COALESCE(SUM(TongTien),0) FROM HoaDon WHERE YEAR(NgayLap)=YEAR(GETDATE()) AND MONTH(NgayLap)=MONTH(GETDATE())",c);return Convert.ToDecimal(cmd.ExecuteScalar());}
    public DataTable Top5(){using var c=Db.Open();using var cmd=new SqlCommand(@"SELECT TOP 5 sp.MaSP,sp.TenSP,SUM(ct.SoLuong) AS DaBan FROM ChiTietHoaDon ct JOIN SanPham sp ON sp.MaSP=ct.MaSP GROUP BY sp.MaSP,sp.TenSP ORDER BY DaBan DESC",c);using var r=cmd.ExecuteReader();var t=new DataTable();t.Load(r);return t;}
}
