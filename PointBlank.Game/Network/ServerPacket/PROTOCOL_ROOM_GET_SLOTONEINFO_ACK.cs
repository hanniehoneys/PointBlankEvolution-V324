using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_GET_SLOTONEINFO_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Account p;
    private PointBlank.Core.Models.Account.Clan.Clan clan;

    public PROTOCOL_ROOM_GET_SLOTONEINFO_ACK(PointBlank.Game.Data.Model.Account player)
    {
      this.p = player;
      if (this.p == null)
        return;
      this.clan = ClanManager.getClan(this.p.clanId);
    }

    public PROTOCOL_ROOM_GET_SLOTONEINFO_ACK(PointBlank.Game.Data.Model.Account player, PointBlank.Core.Models.Account.Clan.Clan c)
    {
      this.p = player;
      this.clan = c;
    }

    public override void write()
    {
      if (this.p._room == null || this.p._slotId == -1)
        return;
      this.writeH((short) 3846);
      this.writeH((short) 0);
      this.writeC((byte) 0);
      this.writeC((byte) this.p._room._slots[this.p._slotId].state);
      this.writeC((byte) this.p.getRank());
      this.writeD(this.clan._id);
      this.writeD(this.p.clanAccess);
      this.writeC((byte) this.clan._rank);
      this.writeD(this.clan._logo);
      this.writeC((byte) this.p.pc_cafe);
      this.writeC((byte) this.p.tourneyLevel);
      this.writeD((uint) this.p.effects);
      this.writeD(0);
      this.writeC((byte) this.clan.effect);
      this.writeUnicode(this.clan._name, 34);
      this.writeC((byte) 0);
      this.writeC((byte) 210);
      this.writeC((byte) this.p._slotId);
      this.writeUnicode(this.p.player_name, 66);
      this.writeC((byte) this.p.name_color);
      this.writeC((byte) this.p._bonus.muzzle);
      this.writeC((byte) 0);
      this.writeC(byte.MaxValue);
      this.writeC(byte.MaxValue);
    }
  }
}
