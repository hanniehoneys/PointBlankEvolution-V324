using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Randombox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PointBlank.Core.Xml
{
  public class RandomBoxXml
  {
    private static SortedList<int, RandomBoxModel> boxes = new SortedList<int, RandomBoxModel>();

    public static void LoadBoxes()
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Data\\Coupons");
      if (!directoryInfo.Exists)
        return;
      foreach (FileInfo file in directoryInfo.GetFiles())
      {
        try
        {
          RandomBoxXml.LoadBox(int.Parse(file.Name.Substring(0, file.Name.Length - 4)));
        }
        catch
        {
        }
      }
    }

    private static void LoadBox(int id)
    {
      string path = "Data/Coupons/" + (object) id + ".xml";
      if (File.Exists(path))
        RandomBoxXml.parse(path, id);
      else
        Logger.error("File not found: " + path);
    }

    private static void parse(string path, int cupomId)
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
                XmlNamedNodeMap attributes1 = (XmlNamedNodeMap) xmlNode1.Attributes;
                RandomBoxModel randomBoxModel = new RandomBoxModel() { itemsCount = int.Parse(attributes1.GetNamedItem("Count").Value) };
                for (XmlNode xmlNode2 = xmlNode1.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
                {
                  if ("Item".Equals(xmlNode2.Name))
                  {
                    XmlNamedNodeMap attributes2 = (XmlNamedNodeMap) xmlNode2.Attributes;
                    ItemsModel itemsModel = (ItemsModel) null;
                    int id = int.Parse(attributes2.GetNamedItem("Id").Value);
                    long num = long.Parse(attributes2.GetNamedItem("Count").Value);
                    if (id > 0)
                      itemsModel = new ItemsModel(id)
                      {
                        _name = "Randombox",
                        _equip = int.Parse(attributes2.GetNamedItem("Equip").Value),
                        _count = num
                      };
                    randomBoxModel.items.Add(new RandomBoxItem()
                    {
                      index = int.Parse(attributes2.GetNamedItem("Index").Value),
                      percent = int.Parse(attributes2.GetNamedItem("Percent").Value),
                      special = bool.Parse(attributes2.GetNamedItem("Special").Value),
                      count = num,
                      item = itemsModel
                    });
                  }
                }
                randomBoxModel.SetTopPercent();
                RandomBoxXml.boxes.Add(cupomId, randomBoxModel);
              }
            }
          }
          catch (Exception ex)
          {
            Logger.error("[Box: " + (object) cupomId + "] " + ex.ToString());
          }
        }
        fileStream.Dispose();
        fileStream.Close();
      }
    }

    public static bool ContainsBox(int id)
    {
      return RandomBoxXml.boxes.ContainsKey(id);
    }

    public static RandomBoxModel getBox(int id)
    {
      try
      {
        return RandomBoxXml.boxes[id];
      }
      catch
      {
        return (RandomBoxModel) null;
      }
    }
  }
}
