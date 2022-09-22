using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Servers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_LOBBY_QUICKJOIN_ROOM_REQ : ReceivePacket
  {
    private List<PointBlank.Game.Data.Model.Room> Rooms = new List<PointBlank.Game.Data.Model.Room>();
    private List<QuickStart> Quicks = new List<QuickStart>();
    private int Select;
    private QuickStart QuickSelect;

    public PROTOCOL_LOBBY_QUICKJOIN_ROOM_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.Select = (int) this.readC();
      for (int index = 0; index < 3; ++index)
        this.Quicks.Add(new QuickStart()
        {
          MapId = (int) this.readC(),
          Rule = (int) this.readC(),
          StageOptions = (int) this.readC(),
          Type = (int) this.readC()
        });
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player == null)
          return;
        Channel channel;
        if (player != null && player.player_name.Length > 0 && (player._room == null && player._match == null) && player.getChannel(out channel))
        {
          lock (channel._rooms)
          {
            for (int index1 = 0; index1 < channel._rooms.Count; ++index1)
            {
              PointBlank.Game.Data.Model.Room room = channel._rooms[index1];
              if (room.room_type != RoomType.Tutorial)
              {
                this.QuickSelect = this.Quicks[this.Select];
                if ((MapIdEnum) this.QuickSelect.MapId == room.mapId && this.QuickSelect.Rule == room.rule && (this.QuickSelect.StageOptions == (int) room.stage && (RoomType) this.QuickSelect.Type == room.room_type) && (room.password.Length == 0 && room.Limit == (byte) 0 && (!room.kickedPlayers.Contains(player.player_id) || player.HaveGMLevel())))
                {
                  for (int index2 = 0; index2 < 16; ++index2)
                  {
                    PointBlank.Core.Models.Room.Slot slot = room._slots[index2];
                    if (slot._playerId == 0L && slot.state == SlotState.EMPTY)
                    {
                      this.Rooms.Add(room);
                      break;
                    }
                  }
                }
              }
            }
          }
        }
        if (this.Rooms.Count == 0)
          this._client.SendPacket((SendPacket) new PROTOCOL_LOBBY_QUICKJOIN_ROOM_ACK(2147483648U, this.Quicks, (QuickStart) null, (PointBlank.Game.Data.Model.Room) null));
        else
          this.getRandomRoom(player);
        this.Rooms = (List<PointBlank.Game.Data.Model.Room>) null;
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_LOBBY_QUICKJOIN_ROOM_REQ: " + ex.ToString());
      }
    }

    private void getRandomRoom(Account player)
    {
      if (player != null)
      {
        PointBlank.Game.Data.Model.Room room = this.Rooms[new Random().Next(this.Rooms.Count)];
        Account p;
        if (room != null && room.getLeader(out p) && room.addPlayer(player) >= 0)
        {
          player.ResetPages();
          using (PROTOCOL_ROOM_GET_SLOTONEINFO_ACK getSlotoneinfoAck = new PROTOCOL_ROOM_GET_SLOTONEINFO_ACK(player))
            room.SendPacketToPlayers((SendPacket) getSlotoneinfoAck, player.player_id);
          this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_JOIN_ACK(0U, player, p));
          this._client.SendPacket((SendPacket) new PROTOCOL_LOBBY_QUICKJOIN_ROOM_ACK(0U, this.Quicks, this.QuickSelect, room));
        }
        else
          this._client.SendPacket((SendPacket) new PROTOCOL_LOBBY_QUICKJOIN_ROOM_ACK(2147483648U, (List<QuickStart>) null, (QuickStart) null, (PointBlank.Game.Data.Model.Room) null));
      }
      else
        this._client.SendPacket((SendPacket) new PROTOCOL_LOBBY_QUICKJOIN_ROOM_ACK(2147483648U, (List<QuickStart>) null, (QuickStart) null, (PointBlank.Game.Data.Model.Room) null));
    }
  }
}
