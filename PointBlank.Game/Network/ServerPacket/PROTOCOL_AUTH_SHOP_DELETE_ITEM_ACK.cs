using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_SHOP_DELETE_ITEM_ACK : SendPacket
  {
    private long _objId;
    private uint _erro;

    public PROTOCOL_AUTH_SHOP_DELETE_ITEM_ACK(uint erro, long objId = 0)
    {
      this._erro = erro;
      if (erro != 1U)
        return;
      this._objId = objId;
    }

    public override void write()
    {
      this.writeH((short) 1056);
      this.writeD(this._erro);
      if (this._erro != 1U)
        return;
      this.writeD((int) this._objId);
    }
  }
}
