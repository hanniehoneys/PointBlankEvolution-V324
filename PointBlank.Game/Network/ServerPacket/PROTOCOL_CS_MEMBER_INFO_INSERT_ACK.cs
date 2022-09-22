using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_MEMBER_INFO_INSERT_ACK : SendPacket
  {
    private Account p;
    private ulong status;

    public PROTOCOL_CS_MEMBER_INFO_INSERT_ACK(Account pl)
    {
      this.p = pl;
      this.status = ComDiv.GetClanStatus(pl._status, pl._isOnline);
    }

    public override void write()
    {
      this.writeH((short) 1871);
      this.writeC((byte) (this.p.player_name.Length + 1));
      this.writeUnicode(this.p.player_name, true);
      this.writeQ(this.p.player_id);
      this.writeQ(this.status);
      this.writeC((byte) this.p._rank);
      this.writeC((byte) this.p.name_color);
    }
  }
}
