using PointBlank.Core.Models.Account.Title;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PointBlank.Core.Xml
{
  public class TitlesXml
  {
    private static List<TitleQ> titles = new List<TitleQ>();

    public static TitleQ getTitle(int titleId, bool ReturnNull = true)
    {
      if (titleId == 0)
      {
        if (!ReturnNull)
          return new TitleQ();
        return (TitleQ) null;
      }
      for (int index = 0; index < TitlesXml.titles.Count; ++index)
      {
        TitleQ title = TitlesXml.titles[index];
        if (title._id == titleId)
          return title;
      }
      return (TitleQ) null;
    }

    public static void get2Titles(
      int titleId1,
      int titleId2,
      out TitleQ title1,
      out TitleQ title2,
      bool ReturnNull = true)
    {
      if (!ReturnNull)
      {
        title1 = new TitleQ();
        title2 = new TitleQ();
      }
      else
      {
        title1 = (TitleQ) null;
        title2 = (TitleQ) null;
      }
      if (titleId1 == 0 && titleId2 == 0)
        return;
      for (int index = 0; index < TitlesXml.titles.Count; ++index)
      {
        TitleQ title = TitlesXml.titles[index];
        if (title._id == titleId1)
          title1 = title;
        else if (title._id == titleId2)
          title2 = title;
      }
    }

    public static void get3Titles(
      int titleId1,
      int titleId2,
      int titleId3,
      out TitleQ title1,
      out TitleQ title2,
      out TitleQ title3,
      bool ReturnNull)
    {
      if (!ReturnNull)
      {
        title1 = new TitleQ();
        title2 = new TitleQ();
        title3 = new TitleQ();
      }
      else
      {
        title1 = (TitleQ) null;
        title2 = (TitleQ) null;
        title3 = (TitleQ) null;
      }
      if (titleId1 == 0 && titleId2 == 0 && titleId3 == 0)
        return;
      for (int index = 0; index < TitlesXml.titles.Count; ++index)
      {
        TitleQ title = TitlesXml.titles[index];
        if (title._id == titleId1)
          title1 = title;
        else if (title._id == titleId2)
          title2 = title;
        else if (title._id == titleId3)
          title3 = title;
      }
    }

    public static void Load()
    {
      string path = "Data/Titles/TitleInfo.xml";
      if (File.Exists(path))
        TitlesXml.parse(path);
      else
        Logger.error("File not found: " + path);
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
                    int titleId = int.Parse(attributes.GetNamedItem("Id").Value);
                    TitlesXml.titles.Add(new TitleQ(titleId)
                    {
                      _classId = int.Parse(attributes.GetNamedItem("List").Value),
                      _medals = int.Parse(attributes.GetNamedItem("Medals").Value),
                      _brooch = int.Parse(attributes.GetNamedItem("Brooch").Value),
                      _blueOrder = int.Parse(attributes.GetNamedItem("BlueOrder").Value),
                      _insignia = int.Parse(attributes.GetNamedItem("Insignia").Value),
                      _rank = int.Parse(attributes.GetNamedItem("Rank").Value),
                      _slot = int.Parse(attributes.GetNamedItem("Slot").Value),
                      _req1 = int.Parse(attributes.GetNamedItem("ReqT1").Value),
                      _req2 = int.Parse(attributes.GetNamedItem("ReqT2").Value)
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
