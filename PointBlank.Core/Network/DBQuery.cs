using System.Collections.Generic;

namespace PointBlank.Core.Network
{
  public class DBQuery
  {
    private List<string> tables;
    private List<object> values;

    public DBQuery()
    {
      this.tables = new List<string>();
      this.values = new List<object>();
    }

    public void AddQuery(string table, object value)
    {
      this.tables.Add(table);
      this.values.Add(value);
    }

    public string[] GetTables()
    {
      return this.tables.ToArray();
    }

    public object[] GetValues()
    {
      return this.values.ToArray();
    }
  }
}
