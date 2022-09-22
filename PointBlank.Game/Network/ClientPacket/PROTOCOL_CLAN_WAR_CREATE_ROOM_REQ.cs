using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CLAN_WAR_CREATE_ROOM_REQ : ReceivePacket
  {
    private int roomId = -1;
    private Match MyMatch;
    private Match EnemyMatch;

    public PROTOCOL_CLAN_WAR_CREATE_ROOM_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      Account player = this._client._player;
      if (player == null || player.clanId == 0)
        return;
      Channel channel = player.getChannel();
      this.MyMatch = player._match;
      if (channel == null || this.MyMatch == null)
        return;
      int id = (int) this.readH();
      this.readD();
      this.readD();
      this.EnemyMatch = channel.getMatch(id);
      try
      {
        if (this.EnemyMatch == null)
          return;
        lock (channel._rooms)
        {
          for (int index = 0; index < 300; ++index)
          {
            if (channel.getRoom(index) == null)
            {
              Room room = new Room(index, channel);
              int num1 = (int) this.readH();
              room.name = this.readS(23);
              room.mapId = (MapIdEnum) this.readH();
              room.stage = this.readC();
              room.room_type = (RoomType) this.readC();
              int num2 = (int) this.readH();
              room.initSlotCount((int) this.readC(), false);
              int num3 = (int) this.readC();
              room.weaponsFlag = (RoomWeaponsFlag) this.readC();
              room.Flag = (RoomStageFlag) this.readC();
              room.password = "";
              room.killtime = 3;
              room.addPlayer(player);
              channel.AddRoom(room);
              this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_CREATE_ACK(0U, room, player));
              this.roomId = index;
              break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    public override void run()
    {
      if (this.roomId == -1)
        return;
      using (PROTOCOL_CLAN_WAR_ENEMY_INFO_ACK clanWarEnemyInfoAck = new PROTOCOL_CLAN_WAR_ENEMY_INFO_ACK(this.EnemyMatch))
      {
        using (PROTOCOL_CLAN_WAR_JOIN_ROOM_ACK clanWarJoinRoomAck = new PROTOCOL_CLAN_WAR_JOIN_ROOM_ACK(this.EnemyMatch, this.roomId, 0))
        {
          byte[] completeBytes1 = clanWarEnemyInfoAck.GetCompleteBytes("PROTOCOL_CLAN_WAR_CREATE_ROOM_REQ-1");
          byte[] completeBytes2 = clanWarJoinRoomAck.GetCompleteBytes("PROTOCOL_CLAN_WAR_CREATE_ROOM_REQ-2");
          for (int index = 0; index < this.MyMatch.getAllPlayers(this.MyMatch._leader).Count; ++index)
          {
            Account allPlayer = this.MyMatch.getAllPlayers(this.MyMatch._leader)[index];
            if (allPlayer._match != null)
            {
              allPlayer.SendCompletePacket(completeBytes1);
              allPlayer.SendCompletePacket(completeBytes2);
              this.MyMatch._slots[allPlayer.matchSlot].state = SlotMatchState.Ready;
            }
          }
        }
      }
      using (PROTOCOL_CLAN_WAR_ENEMY_INFO_ACK clanWarEnemyInfoAck = new PROTOCOL_CLAN_WAR_ENEMY_INFO_ACK(this.MyMatch))
      {
        using (PROTOCOL_CLAN_WAR_JOIN_ROOM_ACK clanWarJoinRoomAck = new PROTOCOL_CLAN_WAR_JOIN_ROOM_ACK(this.MyMatch, this.roomId, 1))
        {
          byte[] completeBytes1 = clanWarEnemyInfoAck.GetCompleteBytes("PROTOCOL_CLAN_WAR_CREATE_ROOM_REQ-3");
          byte[] completeBytes2 = clanWarJoinRoomAck.GetCompleteBytes("PROTOCOL_CLAN_WAR_CREATE_ROOM_REQ-4");
          for (int index = 0; index < this.EnemyMatch.getAllPlayers().Count; ++index)
          {
            Account allPlayer = this.EnemyMatch.getAllPlayers()[index];
            if (allPlayer._match != null)
            {
              allPlayer.SendCompletePacket(completeBytes1);
              allPlayer.SendCompletePacket(completeBytes2);
              this.MyMatch._slots[allPlayer.matchSlot].state = SlotMatchState.Ready;
            }
          }
        }
      }
    }
  }
}
