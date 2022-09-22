using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_ROOM_REQUEST_MAIN_REQ : ReceivePacket
  {
    public PROTOCOL_ROOM_REQUEST_MAIN_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
    }

    public override void run()
    {
      try
      {
        if (this._client == null)
          return;
        Account player = this._client._player;
        Room r = player == null ? (Room) null : player._room;
        if (r != null)
        {
          if (r._state != RoomState.Ready || r._leader == player._slotId)
            return;
          List<Account> allPlayers = r.getAllPlayers();
          if (allPlayers.Count == 0)
            return;
          if (player.access >= AccessLevel.GameMaster)
          {
            this.ChangeLeader(r, allPlayers, player._slotId);
          }
          else
          {
            if (!r.requestHost.Contains(player.player_id))
            {
              r.requestHost.Add(player.player_id);
              if (r.requestHost.Count() < allPlayers.Count / 2 + 1)
              {
                using (PROTOCOL_ROOM_REQUEST_MAIN_ACK roomRequestMainAck = new PROTOCOL_ROOM_REQUEST_MAIN_ACK(player._slotId))
                  this.SendPacketToRoom((SendPacket) roomRequestMainAck, allPlayers);
              }
            }
            if (r.requestHost.Count() < allPlayers.Count / 2 + 1)
              return;
            this.ChangeLeader(r, allPlayers, player._slotId);
          }
        }
        else
          this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_REQUEST_MAIN_ACK(2147483648U));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_ROOM_REQUEST_MAIN_REQ: " + ex.ToString());
      }
    }

    private void ChangeLeader(Room r, List<Account> players, int slotId)
    {
      r.setNewLeader(slotId, 0, -1, false);
      using (PROTOCOL_ROOM_REQUEST_MAIN_CHANGE_WHO_ACK mainChangeWhoAck = new PROTOCOL_ROOM_REQUEST_MAIN_CHANGE_WHO_ACK(slotId))
        this.SendPacketToRoom((SendPacket) mainChangeWhoAck, players);
      r.updateSlotsInfo();
      r.requestHost.Clear();
    }

    private void SendPacketToRoom(SendPacket packet, List<Account> players)
    {
      byte[] completeBytes = packet.GetCompleteBytes(nameof (PROTOCOL_ROOM_REQUEST_MAIN_REQ));
      for (int index = 0; index < players.Count; ++index)
        players[index].SendCompletePacket(completeBytes);
    }
  }
}
