using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Account.Title;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PointBlank.Core.Xml
{
  public class TitleAwardsXml
  {
    public static List<TitleA> awards = new List<TitleA>();

    public static void Load()
    {
      string path = "Data/Titles/TitleAwards.xml";
      if (File.Exists(path))
        TitleAwardsXml.parse(path);
      else
        Logger.error("File not found: " + path);
    }

    public static List<ItemsModel> getAwards(int titleId)
    {
      List<ItemsModel> itemsModelList = new List<ItemsModel>();
      lock (TitleAwardsXml.awards)
      {
        for (int index = 0; index < TitleAwardsXml.awards.Count; ++index)
        {
          TitleA award = TitleAwardsXml.awards[index];
          if (award._id == titleId)
            itemsModelList.Add(award._item);
        }
      }
      return itemsModelList;
    }

    public static bool Contains(int titleId, int itemId)
    {
      if (itemId == 0)
        return false;
      for (int index = 0; index < TitleAwardsXml.awards.Count; ++index)
      {
        TitleA award = TitleAwardsXml.awards[index];
        if (award._id == titleId && award._item._id == itemId)
          return true;
      }
      return false;
    }

    private static void parse(string path)
    {
      XmlDocument xmlDocument = new XmlDocument();
      using (FileStream fileStream = new FileStream(path, FileMode.Open))
      {
        if (fileStream.Length > 0L)
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
                  if ("Title".Equals(xmlNode2.Name))
                  {
                    XmlNamedNodeMap attributes = (XmlNamedNodeMap) xmlNode2.Attributes;
                    int id = int.Parse(attributes.GetNamedItem("ItemId").Value);
                    TitleAwardsXml.awards.Add(new TitleA()
                    {
                      _id = int.Parse(attributes.GetNamedItem("Id").Value),
                      _item = new ItemsModel(id, "Title Reward", int.Parse(attributes.GetNamedItem("Equip").Value), long.Parse(attributes.GetNamedItem("Count").Value), 0L)
                    });
                  }
                }
              }
            }
          }
          catch (XmlException ex)
          {
            Logger.error(ex.ToString());
          }
        }
        fileStream.Dispose();
        fileStream.Close();
      }
    }
  }
}
