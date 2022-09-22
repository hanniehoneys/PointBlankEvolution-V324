using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Map;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_ROOM_CHANGE_SLOT_REQ : ReceivePacket
  {
    private int slotInfo;
    private uint erro;

    public PROTOCOL_ROOM_CHANGE_SLOT_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.slotInfo = this.readD();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        if (room != null && room._leader == player._slotId)
        {
          Slot slot = room.getSlot(this.slotInfo & 268435455);
          if (slot == null)
            return;
          if ((this.slotInfo & 268435456) == 268435456)
            this.OpenSlot(room, slot);
          else
            this.CloseSlot(room, slot);
        }
        else
          this.erro = 2147484673U;
        this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_CHANGE_SLOT_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_ROOM_CHANGE_SLOT_REQ: " + ex.ToString());
      }
    }

    private void CloseSlot(PointBlank.Game.Data.Model.Room room, Slot slot)
    {
      switch (slot.state)
      {
        case SlotState.EMPTY:
          room.changeSlotState(slot, SlotState.CLOSE, true);
          break;
        case SlotState.SHOP:
        case SlotState.INFO:
        case SlotState.CLAN:
        case SlotState.INVENTORY:
        case SlotState.GACHA:
        case SlotState.GIFTSHOP:
        case SlotState.NORMAL:
        case SlotState.READY:
          Account playerBySlot = room.getPlayerBySlot(slot);
          if (playerBySlot == null || playerBySlot.AntiKickGM || (slot.state == SlotState.READY || (room._channelType != 4 || room._state == RoomState.CountDown) && room._channelType == 4) && (slot.state != SlotState.READY || (room._channelType != 4 || room._state != RoomState.Ready) && room._channelType == 4))
            break;
          playerBySlot.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_KICK_PLAYER_ACK());
          room.RemovePlayer(playerBySlot, slot, false, 0);
          break;
      }
    }

    private void OpenSlot(PointBlank.Game.Data.Model.Room room, Slot slot)
    {
      int num = 0;
      for (int index = 0; index < MapModel.Matchs.Count; ++index)
      {
        MapMatch match = MapModel.Matchs[index];
        if ((MapIdEnum) match.Id == room.mapId && MapModel.getRule(match.Mode).Rule == room.rule)
          num = match.Limit;
      }
      if ((this.slotInfo & 268435456) != 268435456 || slot.state != SlotState.CLOSE || (this.slotInfo & 268435455) >= num)
        return;
      room.changeSlotState(slot, SlotState.EMPTY, true);
    }
  }
}
