using PointBlank.Core.Models.Account.Mission;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PointBlank.Core.Xml
{
  public class MissionCardXml
  {
    private static List<MissionItemAward> _items = new List<MissionItemAward>();
    private static List<Card> list = new List<Card>();
    private static List<CardAwards> awards = new List<CardAwards>();

    private static void Load(string file, int type)
    {
      string path = "Data/Missions/" + file + ".mqf";
      if (File.Exists(path))
        MissionCardXml.parse(path, file, type);
      else
        Logger.error("File not found: " + path);
    }

    public static void LoadBasicCards(int type)
    {
      MissionCardXml.Load("AssaultCard", type);
      MissionCardXml.Load("Dino_Basic", type);
      MissionCardXml.Load("Dino_Intensify", type);
      MissionCardXml.Load("TutorialCard_Brazil", type);
      MissionCardXml.Load("Dino_Tutorial", type);
      MissionCardXml.Load("Field_o", type);
      MissionCardXml.Load("SpecialCard", type);
      MissionCardXml.Load("InfiltrationCard", type);
      MissionCardXml.Load("DefconCard", type);
      MissionCardXml.Load("Company_o", type);
      MissionCardXml.Load("BackUpCard", type);
      MissionCardXml.Load("Commissioned_o", type);
      MissionCardXml.Load("EventCard", type);
    }

    private static int ConvertStringToInt(string missionName)
    {
      int num = 0;
      if (missionName == "TutorialCard_Brazil")
        num = 1;
      else if (missionName == "Dino_Tutorial")
        num = 2;
      else if (missionName == "AssaultCard")
        num = 5;
      else if (missionName == "BackUpCard")
        num = 6;
      else if (missionName == "InfiltrationCard")
        num = 7;
      else if (missionName == "SpecialCard")
        num = 8;
      else if (missionName == "DefconCard")
        num = 9;
      else if (missionName == "Commissioned_o")
        num = 10;
      else if (missionName == "Company_o")
        num = 11;
      else if (missionName == "Field_o")
        num = 12;
      else if (missionName == "EventCard")
        num = 13;
      else if (missionName == "Dino_Basic")
        num = 14;
      else if (missionName == "Dino_Intensify")
        num = 16;
      return num;
    }

    public static List<ItemsModel> getMissionAwards(int missionId)
    {
      List<ItemsModel> itemsModelList = new List<ItemsModel>();
      lock (MissionCardXml._items)
      {
        for (int index = 0; index < MissionCardXml._items.Count; ++index)
        {
          MissionItemAward missionItemAward = MissionCardXml._items[index];
          if (missionItemAward._missionId == missionId)
            itemsModelList.Add(missionItemAward.item);
        }
      }
      return itemsModelList;
    }

    public static List<Card> getCards(int missionId, int cardBasicId)
    {
      List<Card> cardList = new List<Card>();
      lock (MissionCardXml.list)
      {
        for (int index = 0; index < MissionCardXml.list.Count; ++index)
        {
          Card card = MissionCardXml.list[index];
          if (card._missionId == missionId && (cardBasicId >= 0 && card._cardBasicId == cardBasicId || cardBasicId == -1))
            cardList.Add(card);
        }
      }
      return cardList;
    }

    public static List<Card> getCards(List<Card> Cards, int cardBasicId)
    {
      if (cardBasicId == -1)
        return Cards;
      List<Card> cardList = new List<Card>();
      for (int index = 0; index < Cards.Count; ++index)
      {
        Card card = Cards[index];
        if (cardBasicId >= 0 && card._cardBasicId == cardBasicId || cardBasicId == -1)
          cardList.Add(card);
      }
      return cardList;
    }

    public static List<Card> getCards(int missionId)
    {
      List<Card> cardList = new List<Card>();
      lock (MissionCardXml.list)
      {
        for (int index = 0; index < MissionCardXml.list.Count; ++index)
        {
          Card card = MissionCardXml.list[index];
          if (card._missionId == missionId)
            cardList.Add(card);
        }
      }
      return cardList;
    }

    private static void parse(string path, string missionName, int typeLoad)
    {
      int num1 = MissionCardXml.ConvertStringToInt(missionName);
      if (num1 == 0)
        Logger.error("Invalid: " + missionName);
      byte[] buff;
      try
      {
        buff = File.ReadAllBytes(path);
      }
      catch
      {
        buff = new byte[0];
      }
      if (buff.Length == 0)
        return;
      try
      {
        ReceiveGPacket receiveGpacket = new ReceiveGPacket(buff);
        receiveGpacket.readS(4);
        int num2 = receiveGpacket.readD();
        receiveGpacket.readB(16);
        int num3 = 0;
        int num4 = 0;
        for (int index = 0; index < 40; ++index)
        {
          int missionBasicId = num4++;
          int cardBasicId = num3;
          if (num4 == 4)
          {
            num4 = 0;
            ++num3;
          }
          int num5 = (int) receiveGpacket.readUH();
          int num6 = (int) receiveGpacket.readC();
          int num7 = (int) receiveGpacket.readC();
          byte num8 = receiveGpacket.readC();
          ClassType classType = (ClassType) receiveGpacket.readC();
          int num9 = (int) receiveGpacket.readUH();
          Card card = new Card(cardBasicId, missionBasicId) { _mapId = num7, _weaponReq = classType, _weaponReqId = num9, _missionType = (MissionType) num6, _missionLimit = (int) num8, _missionId = num1 };
          MissionCardXml.list.Add(card);
          if (num2 == 1)
            receiveGpacket.readB(24);
        }
        int num10 = num2 == 2 ? 5 : 1;
        for (int index1 = 0; index1 < 10; ++index1)
        {
          int num5 = receiveGpacket.readD();
          int num6 = receiveGpacket.readD();
          int medalId = receiveGpacket.readD();
          for (int index2 = 0; index2 < num10; ++index2)
          {
            receiveGpacket.readD();
            receiveGpacket.readD();
            receiveGpacket.readD();
            receiveGpacket.readD();
          }
          if (typeLoad == 1)
          {
            CardAwards card = new CardAwards() { _id = num1, _card = index1, _exp = num2 == 1 ? num6 * 10 : num6, _gp = num5 };
            MissionCardXml.GetCardMedalInfo(card, medalId);
            if (!card.Unusable())
              MissionCardXml.awards.Add(card);
          }
        }
        if (num2 != 2)
          return;
        receiveGpacket.readD();
        receiveGpacket.readB(8);
        for (int index = 0; index < 5; ++index)
        {
          int num5 = receiveGpacket.readD();
          receiveGpacket.readD();
          int id = receiveGpacket.readD();
          int num6 = receiveGpacket.readD();
          if (num5 > 0 && typeLoad == 1)
            MissionCardXml._items.Add(new MissionItemAward()
            {
              _missionId = num1,
              item = new ItemsModel(id)
              {
                _equip = 1,
                _count = (long) num6,
                _name = "Mission Item"
              }
            });
        }
      }
      catch (XmlException ex)
      {
        Logger.error("File error: " + path + "\r\n" + ex.ToString());
      }
    }

    private static void GetCardMedalInfo(CardAwards card, int medalId)
    {
      if (medalId == 0)
        return;
      if (medalId >= 1 && medalId <= 50)
        ++card._brooch;
      else if (medalId >= 51 && medalId <= 100)
      {
        ++card._insignia;
      }
      else
      {
        if (medalId < 101 || medalId > 116)
          return;
        ++card._medal;
      }
    }

    public static CardAwards getAward(int mission, int cartao)
    {
      for (int index = 0; index < MissionCardXml.awards.Count; ++index)
      {
        CardAwards award = MissionCardXml.awards[index];
        if (award._id == mission && award._card == cartao)
          return award;
      }
      return (CardAwards) null;
    }
  }
}
