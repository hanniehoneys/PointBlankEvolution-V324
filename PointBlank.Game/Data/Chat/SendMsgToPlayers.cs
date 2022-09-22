using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Data.Chat
{
  public static class SendMsgToPlayers
  {
    public static string SendToAll(string str)
    {
      string msg = str.Substring(5);
      int num = 0;
      using (PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK messageAnnounceAck = new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(msg))
        num = GameManager.SendPacketToAllClients((SendPacket) messageAnnounceAck);
      return Translation.GetLabel("MsgAllClients", (object) num);
    }

    public static string SendToRoom(string str, Room room)
    {
      string msg = str.Substring(6);
      if (room == null)
        return Translation.GetLabel("GeneralRoomInvalid");
      using (PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK messageAnnounceAck = new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(msg))
        room.SendPacketToPlayers((SendPacket) messageAnnounceAck);
      return Translation.GetLabel("MsgRoomPlayers");
    }
  }
}
