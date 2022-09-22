using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_SELECT_CHANNEL_ACK : SendPacket
  {
    private int _channelId;
    private uint _error;

    public PROTOCOL_BASE_SELECT_CHANNEL_ACK(int id, uint error)
    {
      this._channelId = id;
      this._error = error;
    }

    public PROTOCOL_BASE_SELECT_CHANNEL_ACK(uint erro)
    {
      this._error = erro;
    }

    public override void write()
    {
      this.writeH((short) 543);
      if (this._error == 0U)
      {
        this.writeD(this._error);
        this.writeD(0);
        this.writeH((ushort) this._channelId);
      }
      else
      {
        if (this._error == 0U)
          return;
        this.writeH((short) 0);
        this.writeD(this._error);
      }
    }
  }
}
