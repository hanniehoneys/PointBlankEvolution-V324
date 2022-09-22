using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_JOIN_TEAM_ACK : SendPacket
  {
    private Match m;
    private uint _erro;

    public PROTOCOL_CLAN_WAR_JOIN_TEAM_ACK(uint erro, Match m = null)
    {
      this._erro = erro;
      this.m = m;
    }

    public override void write()
    {
      this.writeH((short) 1549);
      this.writeD(this._erro);
      if (this._erro != 0U)
        return;
      this.writeH((short) this.m._matchId);
      this.writeH((ushort) this.m.getServerInfo());
      this.writeH((ushort) this.m.getServerInfo());
      this.writeC((byte) this.m._state);
      this.writeC((byte) this.m.friendId);
      this.writeC((byte) this.m.formação);
      this.writeC((byte) this.m.getCountPlayers());
      this.writeD(this.m._leader);
      this.writeC((byte) 0);
      this.writeD(this.m.clan._id);
      this.writeC((byte) this.m.clan._rank);
      this.writeD(this.m.clan._logo);
      this.writeS(this.m.clan._name, 17);
      this.writeT(this.m.clan._pontos);
      this.writeC((byte) this.m.clan._name_color);
      for (int index = 0; index < this.m.formação; ++index)
      {
        SlotMatch slot = this.m._slots[index];
        Account playerBySlot = this.m.getPlayerBySlot(slot);
        if (playerBySlot != null)
        {
          this.writeC((byte) playerBySlot._rank);
          this.writeS(playerBySlot.player_name, 33);
          this.writeQ(playerBySlot.player_id);
          this.writeC((byte) slot.state);
        }
        else
          this.writeB(new byte[43]);
      }
    }
  }
}
