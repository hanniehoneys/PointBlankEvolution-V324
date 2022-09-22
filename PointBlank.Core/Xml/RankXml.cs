using Npgsql;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Account.Rank;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;

namespace PointBlank.Core.Xml
{
  public class RankXml
  {
    private static List<RankModel> _ranks = new List<RankModel>();
    private static SortedList<int, List<ItemsModel>> _awards = new SortedList<int, List<ItemsModel>>();

    public static void Load()
    {
      string path = "Data/Rank/Player.xml";
      if (File.Exists(path))
        RankXml.parse(path);
      else
        Logger.error("File not found: " + path);
    }

    public static RankModel getRank(int rankId)
    {
      lock (RankXml._ranks)
      {
        for (int index = 0; index < RankXml._ranks.Count; ++index)
        {
          RankModel rank = RankXml._ranks[index];
          if (rank._id == rankId)
            return rank;
        }
        return (RankModel) null;
      }
    }

    private static void parse(string path)
    {
      XmlDocument xmlDocument = new XmlDocument();
      using (FileStream fileStream = new FileStream(path, FileMode.Open))
      {
        if (fileStream.Length == 0L)
        {
          Logger.error("File is empty: " + path);
        }
        else
        {
          try
          {
            xmlDocument.Load((Stream) fileStream);
            for (XmlNode xmlNode1 = xmlDocument.FirstChild; xmlNode1 != null; xmlNode1 = xmlNode1.NextSibling)
            {
              if ("List".Equals(xmlNode1.Name))
              {
                for (XmlNode xmlNode2 = xmlNode1.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
                {
                  if ("Rank".Equals(xmlNode2.Name))
                  {
                    XmlNamedNodeMap attributes = (XmlNamedNodeMap) xmlNode2.Attributes;
                    RankXml._ranks.Add(new RankModel(int.Parse(attributes.GetNamedItem("Id").Value), int.Parse(attributes.GetNamedItem("NextLevel").Value), int.Parse(attributes.GetNamedItem("PointUp").Value), int.Parse(attributes.GetNamedItem("AllExp").Value)));
                  }
                }
              }
            }
          }
          catch (XmlException ex)
          {
            Logger.warning(ex.ToString());
          }
        }
        fileStream.Dispose();
        fileStream.Close();
      }
    }

    public static List<ItemsModel> getAwards(int rank)
    {
      lock (RankXml._awards)
      {
        List<ItemsModel> itemsModelList;
        if (RankXml._awards.TryGetValue(rank, out itemsModelList))
          return itemsModelList;
      }
      return new List<ItemsModel>();
    }

    public static void LoadAwards()
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandText = "SELECT * FROM info_rank_awards ORDER BY rank_id ASC";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
            RankXml.AddItemToList(npgsqlDataReader.GetInt32(0), new ItemsModel(npgsqlDataReader.GetInt32(1))
            {
              _name = npgsqlDataReader.GetString(2),
              _count = npgsqlDataReader.GetInt64(3),
              _equip = npgsqlDataReader.GetInt32(4)
            });
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

    private static void AddItemToList(int rank, ItemsModel item)
    {
      if (RankXml._awards.ContainsKey(rank))
        RankXml._awards[rank].Add(item);
      else
        RankXml._awards.Add(rank, new List<ItemsModel>()
        {
          item
        });
    }
  }
}
