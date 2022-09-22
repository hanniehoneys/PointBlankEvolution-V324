using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Sync.Client;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_MISSION_DEFENCE_INFO_REQ : ReceivePacket
  {
    private List<ushort> _damag1 = new List<ushort>();
    private List<ushort> _damag2 = new List<ushort>();
    private ushort tanqueA;
    private ushort tanqueB;

    public PROTOCOL_BATTLE_MISSION_DEFENCE_INFO_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.tanqueA = this.readUH();
      this.tanqueB = this.readUH();
      for (int index = 0; index < 16; ++index)
        this._damag1.Add(this.readUH());
      for (int index = 0; index < 16; ++index)
        this._damag2.Add(this.readUH());
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        if (room == null || room.round.Timer != null || (room._state != RoomState.Battle || room.swapRound))
          return;
        PointBlank.Core.Models.Room.Slot slot1 = room.getSlot(player._slotId);
        if (slot1 == null || slot1.state != SlotState.BATTLE)
          return;
        room.Bar1 = (int) this.tanqueA;
        room.Bar2 = (int) this.tanqueB;
        for (int index = 0; index < 16; ++index)
        {
          PointBlank.Core.Models.Room.Slot slot2 = room._slots[index];
          if (slot2._playerId > 0L && slot2.state == SlotState.BATTLE)
          {
            slot2.damageBar1 = this._damag1[index];
            slot2.damageBar2 = this._damag2[index];
          }
        }
        using (PROTOCOL_BATTLE_MISSION_DEFENCE_INFO_ACK missionDefenceInfoAck = new PROTOCOL_BATTLE_MISSION_DEFENCE_INFO_ACK(room))
          room.SendPacketToPlayers((SendPacket) missionDefenceInfoAck, SlotState.BATTLE, 0);
        if (this.tanqueA != (ushort) 0 || this.tanqueB != (ushort) 0)
          return;
        RoomSabotageSync.EndRound(room, (byte) 0);
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }
  }
}
