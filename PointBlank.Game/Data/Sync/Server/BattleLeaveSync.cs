using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Data.Sync.Server
{
  public class BattleLeaveSync
  {
    public static void SendUDPPlayerLeave(Room room, int slotId)
    {
      try
      {
        if (room == null)
          return;
        int playingPlayers = room.getPlayingPlayers(2, SlotState.BATTLE, 0, slotId);
        using (SendGPacket sendGpacket = new SendGPacket())
        {
          sendGpacket.writeH((short) 2);
          sendGpacket.writeD(room.UniqueRoomId);
          sendGpacket.writeD(room.Seed);
          sendGpacket.writeC((byte) slotId);
          sendGpacket.writeC((byte) playingPlayers);
          GameSync.SendPacket(sendGpacket.mstream.ToArray(), room.UdpServer.Connection);
        }
      }
      catch
      {
      }
    }
  }
}
