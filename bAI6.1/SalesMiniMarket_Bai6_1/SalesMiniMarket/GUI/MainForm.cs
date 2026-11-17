using SalesMiniMarket.BUS;
using SalesMiniMarket.DTO;
using System.Data;

namespace SalesMiniMarket.GUI;
public class MainForm : Form
{
    ProductService products=new(); CustomerService customers=new(); EmployeeService employees=new(); SalesService sales=new();
    TabControl tabs=new(){Dock=DockStyle.Fill};
    DataGridView productGrid=Grid(), customerGrid=Grid(), employeeGrid=Grid(), cartGrid=Grid(), topGrid=Grid();
    TextBox pCode=new(),pName=new(),pPrice=new(),pStock=new(),pSearch=new();
    TextBox cName=new(),cPhone=new(),eName=new(),ePhone=new(),eRole=new(),saleCode=new();
    NumericUpDown saleQty=new(){Minimum=1,Maximum=100000,Value=1,Width=90};
    ComboBox saleEmployee=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=220},saleCustomer=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=250};
    Label totalLabel=new(){AutoSize=true,Font=new Font("Segoe UI",12,FontStyle.Bold)};
    List<CartItem> cart=new();
    public MainForm(){Text="RETAIL SALES SYSTEM - Quản lý siêu thị mini";Width=1200;Height=760;StartPosition=FormStartPosition.CenterScreen;Controls.Add(tabs);BuildProducts();BuildCustomers();BuildEmployees();BuildSales();BuildReports();ReloadAll();}
    static DataGridView Grid()=>new(){Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
    Button Btn(string text,Action action){var b=new Button{Text=text,AutoSize=true,Padding=new Padding(8,4,8,4)};b.Click+=(_,_)=>Run(action);return b;}
    void Run(Action a){try{a();}catch(Exception ex){MessageBox.Show(ex.Message,"Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
    FlowLayoutPanel Bar()=>new(){Dock=DockStyle.Top,AutoSize=true,Padding=new Padding(8),WrapContents=true};
    void AddTab(string name,Control c){var page=new TabPage(name);page.Controls.Add(c);tabs.TabPages.Add(page);}
    void BuildProducts(){
        var panel=new Panel{Dock=DockStyle.Fill};var bar=Bar();
        foreach(var (label,box) in new[]{("Mã SP",pCode),("Tên SP",pName),("Giá bán",pPrice),("Tồn kho",pStock),("Tìm kiếm",pSearch)}){bar.Controls.Add(new Label{Text=label,AutoSize=true,Padding=new Padding(3,8,0,0)});box.Width=125;bar.Controls.Add(box);}
        bar.Controls.Add(Btn("Tìm",()=>productGrid.DataSource=products.GetAll(pSearch.Text)));
        bar.Controls.Add(Btn("Thêm",()=>{products.Save(ReadProduct(),true);ReloadProducts();}));
        bar.Controls.Add(Btn("Sửa",()=>{products.Save(ReadProduct(),false);ReloadProducts();}));
        bar.Controls.Add(Btn("Xóa",()=>{products.Delete(pCode.Text);ReloadProducts();}));
        bar.Controls.Add(Btn("Làm mới",()=>{pCode.Clear();pName.Clear();pPrice.Clear();pStock.Clear();ReloadProducts();}));
        productGrid.SelectionChanged+=(_,_)=>{if(productGrid.CurrentRow?.DataBoundItem is DataRowView r){pCode.Text=r["MaSP"].ToString();pName.Text=r["TenSP"].ToString();pPrice.Text=r["GiaBan"].ToString();pStock.Text=r["SoLuongTon"].ToString();}};
        panel.Controls.Add(productGrid);panel.Controls.Add(bar);AddTab("Sản phẩm",panel);
    }
    SanPham ReadProduct()=>new(){MaSP=pCode.Text.Trim(),TenSP=pName.Text.Trim(),GiaBan=decimal.Parse(pPrice.Text),SoLuongTon=int.Parse(pStock.Text)};
    void BuildCustomers(){
        var panel=new Panel{Dock=DockStyle.Fill};var bar=Bar();
        foreach(var (s,b) in new[]{("Tên KH",cName),("Điện thoại",cPhone)}){bar.Controls.Add(new Label{Text=s,AutoSize=true,Padding=new Padding(3,8,0,0)});b.Width=160;bar.Controls.Add(b);}
        bar.Controls.Add(Btn("Thêm",()=>{customers.Save(new(){TenKH=cName.Text,DienThoai=cPhone.Text},true);ReloadCustomers();}));
        bar.Controls.Add(Btn("Sửa",()=>{int id=SelectedId(customerGrid,"MaKH");customers.Save(new(){MaKH=id,TenKH=cName.Text,DienThoai=cPhone.Text},false);ReloadCustomers();}));
        bar.Controls.Add(Btn("Xóa",()=>{customers.Delete(SelectedId(customerGrid,"MaKH"));ReloadCustomers();}));
        customerGrid.SelectionChanged+=(_,_)=>{if(customerGrid.CurrentRow?.DataBoundItem is DataRowView r){cName.Text=r["TenKH"].ToString();cPhone.Text=r["DienThoai"].ToString();}};
        panel.Controls.Add(customerGrid);panel.Controls.Add(bar);AddTab("Khách hàng",panel);
    }
    void BuildEmployees(){
        var panel=new Panel{Dock=DockStyle.Fill};var bar=Bar();
        foreach(var (s,b) in new[]{("Tên NV",eName),("Điện thoại",ePhone),("Chức vụ",eRole)}){bar.Controls.Add(new Label{Text=s,AutoSize=true,Padding=new Padding(3,8,0,0)});b.Width=150;bar.Controls.Add(b);}
        bar.Controls.Add(Btn("Thêm",()=>{employees.Save(new(){TenNV=eName.Text,SoDienThoai=ePhone.Text,ChucVu=eRole.Text},true);ReloadEmployees();}));
        bar.Controls.Add(Btn("Sửa",()=>{int id=SelectedId(employeeGrid,"MaNV");employees.Save(new(){MaNV=id,TenNV=eName.Text,SoDienThoai=ePhone.Text,ChucVu=eRole.Text},false);ReloadEmployees();}));
        bar.Controls.Add(Btn("Xóa",()=>{employees.Delete(SelectedId(employeeGrid,"MaNV"));ReloadEmployees();}));
        employeeGrid.SelectionChanged+=(_,_)=>{if(employeeGrid.CurrentRow?.DataBoundItem is DataRowView r){eName.Text=r["TenNV"].ToString();ePhone.Text=r["SoDienThoai"].ToString();eRole.Text=r["ChucVu"].ToString();}};
        panel.Controls.Add(employeeGrid);panel.Controls.Add(bar);AddTab("Nhân viên",panel);
    }
    int SelectedId(DataGridView g,string col)=>Convert.ToInt32(g.CurrentRow?.Cells[col].Value??throw new InvalidOperationException("Hãy chọn một dòng."));
    void BuildSales(){
        var panel=new Panel{Dock=DockStyle.Fill};var bar=Bar();
        bar.Controls.Add(new Label{Text="Nhân viên",AutoSize=true,Padding=new Padding(3,8,0,0)});bar.Controls.Add(saleEmployee);
        bar.Controls.Add(new Label{Text="Khách hàng",AutoSize=true,Padding=new Padding(3,8,0,0)});bar.Controls.Add(saleCustomer);
        bar.Controls.Add(new Label{Text="Mã SP",AutoSize=true,Padding=new Padding(3,8,0,0)});saleCode.Width=130;bar.Controls.Add(saleCode);
        bar.Controls.Add(new Label{Text="Số lượng",AutoSize=true,Padding=new Padding(3,8,0,0)});bar.Controls.Add(saleQty);
        bar.Controls.Add(Btn("Thêm vào giỏ",AddToCart));bar.Controls.Add(Btn("Xóa dòng",()=>{if(cartGrid.CurrentRow!=null){cart.RemoveAt(cartGrid.CurrentRow.Index);RefreshCart();}}));
        var bottom=Bar();bottom.Controls.Add(totalLabel);bottom.Controls.Add(Btn("Thanh toán",Checkout));
        panel.Controls.Add(cartGrid);panel.Controls.Add(bottom);panel.Controls.Add(bar);AddTab("Bán hàng",panel);
    }
    void AddToCart(){
        var p=products.Find(saleCode.Text.Trim())??throw new InvalidOperationException("Không tìm thấy sản phẩm.");
        int q=(int)saleQty.Value;if(q>p.SoLuongTon)throw new InvalidOperationException("Số lượng vượt tồn kho.");
        var old=cart.FirstOrDefault(x=>x.MaSP==p.MaSP);
        if(old!=null){if(old.SoLuong+q>p.SoLuongTon)throw new InvalidOperationException("Số lượng trong giỏ vượt tồn kho.");old.SoLuong+=q;}
        else cart.Add(new(){MaSP=p.MaSP,TenSP=p.TenSP,SoLuong=q,DonGia=p.GiaBan});
        RefreshCart();
    }
    void RefreshCart(){cartGrid.DataSource=null;cartGrid.DataSource=cart.Select(x=>new{ x.MaSP,x.TenSP,x.SoLuong,x.DonGia,ThanhTien=x.ThanhTien}).ToList();totalLabel.Text=$"Tổng tiền: {cart.Sum(x=>x.ThanhTien):N0} đ";}
    void Checkout(){
        if(saleEmployee.SelectedItem is not NhanVien nv)throw new InvalidOperationException("Hãy chọn nhân viên.");
        int? kh=(saleCustomer.SelectedItem is KhachHang customer)?customer.MaKH:null;
        sales.Checkout(nv.MaNV,kh,cart);
        MessageBox.Show("Thanh toán thành công!");cart.Clear();RefreshCart();ReloadProducts();ReloadReports();
    }
    void BuildReports(){
        var panel=new Panel{Dock=DockStyle.Fill};var bar=Bar();var today=new Label{AutoSize=true,Padding=new Padding(5)};var month=new Label{AutoSize=true,Padding=new Padding(5)};
        bar.Controls.Add(today);bar.Controls.Add(month);bar.Controls.Add(Btn("Cập nhật báo cáo",()=>ReloadReports(today,month)));
        panel.Controls.Add(topGrid);panel.Controls.Add(bar);AddTab("Báo cáo",panel);
        void init(){ReloadReports(today,month);} Shown+=(_,_)=>init();
    }
    void ReloadReports(Label? today=null,Label? month=null){
        if(today!=null)today.Text=$"Doanh thu hôm nay: {sales.RevenueToday():N0} đ";
        if(month!=null)month.Text=$"Doanh thu tháng này: {sales.RevenueMonth():N0} đ";
        topGrid.DataSource=sales.Top5();
    }
    void ReloadProducts()=>productGrid.DataSource=products.GetAll(pSearch.Text);
    void ReloadCustomers()=>customerGrid.DataSource=customers.GetAll();
    void ReloadEmployees()=>employeeGrid.DataSource=employees.GetAll();
    void ReloadAll(){
        Run(()=>{ReloadProducts();ReloadCustomers();ReloadEmployees();saleEmployee.DataSource=employees.GetList();saleEmployee.DisplayMember="TenNV";saleCustomer.DataSource=new[]{new KhachHang{MaKH=0,TenKH="Khách lẻ"}}.Concat(customers.GetList()).ToList();saleCustomer.DisplayMember="TenKH";saleCustomer.SelectedIndex=0;RefreshCart();ReloadReports();});
    }
}
