using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_PRESTARTBATTLE_ACK : SendPacket
  {
    private Account Player;
    private Account Leader;
    private Room Room;
    private bool isPreparing;
    private bool LoadHits;
    private uint UniqueRoomId;
    private uint Seed;

    public PROTOCOL_BATTLE_PRESTARTBATTLE_ACK(Account Player, Account Leader, bool LoadHits)
    {
      this.Player = Player;
      this.Leader = Leader;
      this.LoadHits = LoadHits;
      this.Room = Player._room;
      if (this.Room == null)
        return;
      this.isPreparing = this.Room.isPreparing();
      this.UniqueRoomId = this.Room.UniqueRoomId;
      this.Seed = this.Room.Seed;
    }

    public PROTOCOL_BATTLE_PRESTARTBATTLE_ACK()
    {
    }

    public override void write()
    {
      this.writeH((short) 4106);
      this.writeD(this.isPreparing);
      if (!this.isPreparing)
        return;
      this.writeD(this.Player._slotId);
      if (this.Room.room_type != RoomType.Tutorial)
        this.writeC((byte) GameConfig.udpType);
      else
        this.writeC((byte) 1);
      this.writeIP(this.Room.UdpServer.Connection.Address);
      this.writeH((ushort) this.Room.UdpServer.Port);
      this.writeD(this.UniqueRoomId);
      this.writeD(this.Seed);
      if (!this.LoadHits)
        return;
      this.writeB(new byte[35]
      {
        (byte) 21,
        (byte) 10,
        (byte) 11,
        (byte) 12,
        (byte) 13,
        (byte) 14,
        (byte) 6,
        (byte) 16,
        (byte) 17,
        (byte) 18,
        (byte) 15,
        (byte) 20,
        (byte) 33,
        (byte) 22,
        (byte) 3,
        (byte) 19,
        (byte) 25,
        (byte) 26,
        (byte) 27,
        (byte) 28,
        (byte) 29,
        (byte) 30,
        (byte) 31,
        (byte) 32,
        (byte) 9,
        (byte) 34,
        (byte) 0,
        (byte) 1,
        (byte) 2,
        (byte) 23,
        (byte) 4,
        (byte) 5,
        (byte) 24,
        (byte) 7,
        (byte) 8
      });
    }
  }
}
