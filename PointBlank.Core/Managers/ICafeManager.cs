using Npgsql;
using PointBlank.Core.Models;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Core.Managers
{
  public static class ICafeManager
  {
    public static List<ICafe> GetList()
    {
      List<ICafe> icafeList = new List<ICafe>();
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandText = "SELECT * FROM pc_icafe";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            ICafe icafe = new ICafe(npgsqlDataReader.GetString(2));
            icafeList.Add(icafe);
          }
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
      return icafeList;
    }

    public static bool GetCafe(string Ip)
    {
      bool flag = false;
      if (Ip == "")
        flag = false;
      for (int index = 0; index < ICafeManager.GetList().Count; ++index)
      {
        ICafe icafe = ICafeManager.GetList()[index];
        flag = Ip == icafe.Ip;
      }
      return flag;
    }
  }
}
