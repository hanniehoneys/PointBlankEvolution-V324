using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Network;

namespace PointBlank.Battle.Data.Sync.Client
{
  public static class RemovePlayerSync
  {
    public static void Load(ReceivePacket p)
    {
      uint UniqueRoomId = p.readUD();
      uint Seed = p.readUD();
      int Slot = (int) p.readC();
      int num = (int) p.readC();
      Room room = RoomsManager.getRoom(UniqueRoomId, Seed);
      if (room == null)
        return;
      if (num == 0)
      {
        RoomsManager.RemoveRoom(UniqueRoomId);
      }
      else
      {
        Player player = room.getPlayer(Slot, false);
        if (player != null)
          player.ResetAllInfos();
      }
    }
  }
}
