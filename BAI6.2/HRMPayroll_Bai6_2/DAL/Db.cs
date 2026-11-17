using Microsoft.Data.SqlClient;
using System.Configuration;
namespace HRMPayroll.DAL;
public static class Db
{
 public static string ConnectionString=>ConfigurationManager.ConnectionStrings["HRMDB"]?.ConnectionString
 ?? "Server=.;Database=HRMDB;Trusted_Connection=True;TrustServerCertificate=True;";
 public static SqlConnection Open(){var c=new SqlConnection(ConnectionString);c.Open();return c;}
}