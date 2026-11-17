using SalesMiniMarket.DAL;
using SalesMiniMarket.DTO;
using System.Data;
namespace SalesMiniMarket.BUS;
public class ProductService
{
    private readonly ProductRepository repo=new();
    public DataTable GetAll(string key="")=>repo.GetAll(key);
    public void Save(SanPham p,bool insert){if(string.IsNullOrWhiteSpace(p.MaSP)||string.IsNullOrWhiteSpace(p.TenSP)||p.GiaBan<=0||p.SoLuongTon<0)throw new ArgumentException("Vui lòng nhập dữ liệu hợp lệ (giá > 0, tồn kho >= 0).");repo.Save(p,insert);}
    public void Delete(string id)=>repo.Delete(id);
    public SanPham? Find(string id)=>repo.Find(id);
}
public class CustomerService
{
    private readonly CustomerRepository repo=new();
    public DataTable GetAll()=>repo.GetAll(); public List<KhachHang> GetList()=>repo.GetList();
    public void Save(KhachHang x,bool insert){if(string.IsNullOrWhiteSpace(x.TenKH))throw new ArgumentException("Tên khách hàng không được trống.");repo.Save(x,insert);}
    public void Delete(int id)=>repo.Delete(id);
}
public class EmployeeService
{
    private readonly EmployeeRepository repo=new();
    public DataTable GetAll()=>repo.GetAll(); public List<NhanVien> GetList()=>repo.GetList();
    public void Save(NhanVien x,bool insert){if(string.IsNullOrWhiteSpace(x.TenNV))throw new ArgumentException("Tên nhân viên không được trống.");repo.Save(x,insert);}
    public void Delete(int id)=>repo.Delete(id);
}
public class SalesService
{
    private readonly SalesRepository repo=new();
    public void Checkout(int nv,int? kh,List<CartItem> items)=>repo.Checkout(nv,kh,items);
    public decimal RevenueToday()=>repo.RevenueToday(); public decimal RevenueMonth()=>repo.RevenueMonth(); public DataTable Top5()=>repo.Top5();
}
