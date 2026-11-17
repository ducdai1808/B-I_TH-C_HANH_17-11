using Microsoft.Data.SqlClient;
using System.Configuration;
namespace SalesMiniMarket.DAL;
public static class Db
{
    public static string ConnectionString =>
        ConfigurationManager.ConnectionStrings["SalesDB"]?.ConnectionString
        ?? "Server=.;Database=SalesDB;Trusted_Connection=True;TrustServerCertificate=True;";
    public static SqlConnection Open()
    {
        var c = new SqlConnection(ConnectionString);
        c.Open();
        return c;
    }
}
