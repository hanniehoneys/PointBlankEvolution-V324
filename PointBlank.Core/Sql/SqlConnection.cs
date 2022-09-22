using Npgsql;
using System.Runtime.Remoting.Contexts;

namespace PointBlank.Core.Sql
{
  [Synchronization]
  public class SqlConnection
  {
    private static SqlConnection sql = new SqlConnection();
    protected NpgsqlConnectionStringBuilder connBuilder;

    public SqlConnection()
    {
      this.connBuilder = new NpgsqlConnectionStringBuilder()
      {
        Database = Config.dbName,
        Host = Config.dbHost,
        Username = Config.dbUser,
        Password = Config.dbPass,
        Port = Config.dbPort
      };
    }

    public static SqlConnection getInstance()
    {
      return SqlConnection.sql;
    }

    public NpgsqlConnection conn()
    {
      return new NpgsqlConnection(this.connBuilder.ConnectionString);
    }
  }
}
