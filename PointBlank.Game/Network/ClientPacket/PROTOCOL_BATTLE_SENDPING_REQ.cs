using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_SENDPING_REQ : ReceivePacket
  {
    private byte[] slots;
    private int ReadyPlayersCount;

    public PROTOCOL_BATTLE_SENDPING_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.slots = this.readB(16);
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player == null)
          return;
        Room room = player._room;
        if (room == null || room._slots[player._slotId].state < SlotState.BATTLE_READY)
          return;
        if (room._state == RoomState.Battle)
          room._ping = 5;
        using (PROTOCOL_BATTLE_SENDPING_ACK battleSendpingAck = new PROTOCOL_BATTLE_SENDPING_ACK(this.slots))
        {
          List<Account> allPlayers = room.getAllPlayers(SlotState.READY, 1);
          if (allPlayers.Count == 0)
            return;
          byte[] completeBytes = battleSendpingAck.GetCompleteBytes(nameof (PROTOCOL_BATTLE_SENDPING_REQ));
          foreach (Account account in allPlayers)
          {
            if (room._slots[account._slotId].state >= SlotState.BATTLE_READY)
              account.SendCompletePacket(completeBytes);
            else
              ++this.ReadyPlayersCount;
          }
        }
        if (this.ReadyPlayersCount != 0)
          return;
        room.SpawnReadyPlayers();
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_BATTLE_SENDPING_REQ: " + ex.ToString());
      }
    }
  }
}
