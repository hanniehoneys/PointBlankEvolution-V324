using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Sync.Client;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_MISSION_BOMB_INSTALL_REQ : ReceivePacket
  {
    private int slotIdx;
    private float x;
    private float y;
    private float z;
    private byte area;

    public PROTOCOL_BATTLE_MISSION_BOMB_INSTALL_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.slotIdx = this.readD();
      this.area = this.readC();
      this.x = this.readT();
      this.y = this.readT();
      this.z = this.readT();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        if (room == null || room.round.Timer != null || (room._state != RoomState.Battle || room.C4_actived))
          return;
        Slot slot = room.getSlot(this.slotIdx);
        if (slot == null || slot.state != SlotState.BATTLE)
          return;
        RoomC4.InstallBomb(room, slot, (int) this.area, this.x, this.y, this.z);
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BATTLE_MISSION_BOMB_INSTALL_REQ: " + ex.ToString());
      }
    }
  }
}
