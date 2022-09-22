using Npgsql;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Core.Xml
{
  public class BasicInventoryXml
  {
    public static List<ItemsModel> basic = new List<ItemsModel>();
    public static List<ItemsModel> creationAwards = new List<ItemsModel>();
    public static List<ItemsModel> Characters = new List<ItemsModel>();

    public static void Load()
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandText = "SELECT * FROM info_basic_items";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            int int32 = npgsqlDataReader.GetInt32(0);
            ItemsModel itemsModel = new ItemsModel(npgsqlDataReader.GetInt32(1)) { _name = npgsqlDataReader.GetString(2), _count = npgsqlDataReader.GetInt64(3), _equip = npgsqlDataReader.GetInt32(4) };
            switch (int32)
            {
              case 0:
                BasicInventoryXml.basic.Add(itemsModel);
                break;
              case 1:
                BasicInventoryXml.creationAwards.Add(itemsModel);
                break;
              case 2:
                BasicInventoryXml.Characters.Add(itemsModel);
                break;
            }
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
    }
  }
}
