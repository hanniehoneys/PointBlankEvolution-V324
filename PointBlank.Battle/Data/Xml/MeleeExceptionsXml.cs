using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PointBlank.Battle.Data.Xml
{
  public class MeleeExceptionsXml
  {
    public static List<MeleeExcep> _items = new List<MeleeExcep>();

    public static bool Contains(int number)
    {
      for (int index = 0; index < MeleeExceptionsXml._items.Count; ++index)
      {
        if (MeleeExceptionsXml._items[index].Number == number)
          return true;
      }
      return false;
    }

    public static void Load()
    {
      string path = "Data/Battle/Exceptions.xml";
      if (File.Exists(path))
        MeleeExceptionsXml.parse(path);
      else
        Logger.warning("File not found: " + path);
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
                  if ("Weapon".Equals(xmlNode2.Name))
                  {
                    XmlNamedNodeMap attributes = (XmlNamedNodeMap) xmlNode2.Attributes;
                    MeleeExcep meleeExcep = new MeleeExcep() { Number = int.Parse(attributes.GetNamedItem("Number").Value) };
                    MeleeExceptionsXml._items.Add(meleeExcep);
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
      Logger.info("Loaded: " + (object) MeleeExceptionsXml._items.Count + " melee exceptions");
    }
  }
}
