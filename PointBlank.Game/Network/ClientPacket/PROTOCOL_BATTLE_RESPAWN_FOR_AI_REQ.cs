using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_RESPAWN_FOR_AI_REQ : ReceivePacket
  {
    private int slotIdx;

    public PROTOCOL_BATTLE_RESPAWN_FOR_AI_REQ(GameClient gc, byte[] data)
    {
      this.makeme(gc, data);
    }

    public override void read()
    {
      this.slotIdx = this.readD();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player == null)
          return;
        Room room = player._room;
        if (room == null || room._state != RoomState.Battle || player._slotId != room._leader)
          return;
        room.getSlot(this.slotIdx).aiLevel = (int) room.IngameAiLevel;
        ++room.spawnsCount;
        using (PROTOCOL_BATTLE_RESPAWN_FOR_AI_ACK battleRespawnForAiAck = new PROTOCOL_BATTLE_RESPAWN_FOR_AI_ACK(this.slotIdx))
          room.SendPacketToPlayers((SendPacket) battleRespawnForAiAck, SlotState.BATTLE, 0);
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BATTLE_RESPAWN_FOR_AI_REQ: " + ex.ToString());
      }
    }
  }
}
