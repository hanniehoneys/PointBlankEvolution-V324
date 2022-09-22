using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_MISSION_ROUND_END_ACK : SendPacket
  {
    private Room _room;
    private int _winner;
    private RoundEndType _reason;

    public PROTOCOL_BATTLE_MISSION_ROUND_END_ACK(Room room, int winner, RoundEndType reason)
    {
      this._room = room;
      this._winner = winner;
      this._reason = reason;
    }

    public PROTOCOL_BATTLE_MISSION_ROUND_END_ACK(
      Room room,
      TeamResultType winner,
      RoundEndType reason)
    {
      this._room = room;
      this._winner = (int) winner;
      this._reason = reason;
    }

    public override void write()
    {
      this.writeH((short) 4131);
      this.writeC((byte) this._winner);
      this.writeC((byte) this._reason);
      if (this._room.room_type == RoomType.Boss)
      {
        this.writeH((ushort) this._room.red_dino);
        this.writeH((ushort) this._room.blue_dino);
      }
      else if (this._room.room_type == RoomType.DeathMatch || this._room.room_type == RoomType.FreeForAll)
      {
        this.writeH((ushort) this._room._redKills);
        this.writeH((ushort) this._room._blueKills);
      }
      else
      {
        this.writeH((ushort) this._room.red_rounds);
        this.writeH((ushort) this._room.blue_rounds);
      }
    }
  }
}
