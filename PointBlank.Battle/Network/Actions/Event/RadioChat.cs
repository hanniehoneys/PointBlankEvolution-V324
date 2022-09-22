using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Data.Models.Event;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class RadioChat
  {
    public static RadioChatInfo ReadSyncInfo(
      ActionModel ac,
      ReceivePacket p,
      bool genLog)
    {
      RadioChatInfo radioChatInfo = new RadioChatInfo() { RadioId = p.readC(), AreaId = p.readC() };
      if (genLog)
        Logger.warning("Slot: " + (object) ac.Slot + " Radio: " + (object) radioChatInfo.RadioId + " Area: " + (object) radioChatInfo.AreaId);
      return radioChatInfo;
    }

    public static void WriteInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
    {
      RadioChatInfo radioChatInfo = RadioChat.ReadSyncInfo(ac, p, genLog);
      s.writeC(radioChatInfo.RadioId);
      s.writeC(radioChatInfo.AreaId);
    }
  }
}
