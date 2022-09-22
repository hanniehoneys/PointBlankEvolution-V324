using PointBlank.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PointBlank.Game.Data.Xml
{
  public static class BattleServerXml
  {
    public static List<BattleServer> Servers = new List<BattleServer>();

    public static void Load()
    {
      string path = "Data/Battle/ServerList.xml";
      if (File.Exists(path))
        BattleServerXml.parse(path);
      else
        Logger.error("File not found: " + path);
    }

    public static BattleServer GetRandomServer()
    {
      if (BattleServerXml.Servers.Count == 0)
        return (BattleServer) null;
      int index = new Random().Next(BattleServerXml.Servers.Count);
      try
      {
        return BattleServerXml.Servers[index];
      }
      catch
      {
        return (BattleServer) null;
      }
    }

    private static void parse(string path)
    {
      XmlDocument xmlDocument = new XmlDocument();
      try
      {
        using (FileStream fileStream = new FileStream(path, FileMode.Open))
        {
          if (fileStream.Length > 0L)
          {
            xmlDocument.Load((Stream) fileStream);
            for (XmlNode xmlNode1 = xmlDocument.FirstChild; xmlNode1 != null; xmlNode1 = xmlNode1.NextSibling)
            {
              if ("List".Equals(xmlNode1.Name))
              {
                XmlNamedNodeMap attributes1 = (XmlNamedNodeMap) xmlNode1.Attributes;
                for (XmlNode xmlNode2 = xmlNode1.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
                {
                  if ("Server".Equals(xmlNode2.Name))
                  {
                    XmlNamedNodeMap attributes2 = (XmlNamedNodeMap) xmlNode2.Attributes;
                    BattleServer battleServer = new BattleServer(attributes2.GetNamedItem("Ip").Value, int.Parse(attributes2.GetNamedItem("Sync").Value))
                    {
                      Port = int.Parse(attributes2.GetNamedItem("Port").Value)
                    };
                    BattleServerXml.Servers.Add(battleServer);
                  }
                }
              }
            }
          }
          fileStream.Dispose();
          fileStream.Close();
        }
      }
      catch (XmlException ex)
      {
        Logger.error("File error: " + path + "\r\n" + ex.ToString());
      }
    }
  }
}
