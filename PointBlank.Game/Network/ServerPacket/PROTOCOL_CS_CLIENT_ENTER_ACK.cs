using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CLIENT_ENTER_ACK : SendPacket
  {
    private int _type;
    private int _clanId;

    public PROTOCOL_CS_CLIENT_ENTER_ACK(int id, int access)
    {
      this._clanId = id;
      this._type = access;
    }

    public override void write()
    {
      this.writeH((short) 1794);
      this.writeD(this._clanId);
      this.writeD(this._type);
      if (this._clanId != 0 && this._type != 0)
        return;
      this.writeD(ClanManager._clans.Count);
      this.writeC((byte) 15);
      this.writeH((ushort) Math.Ceiling((double) ClanManager._clans.Count / 15.0));
      this.writeD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
    }
  }
}
