using PointBlank.Core.Network;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_MESSENGER_NOTE_DELETE_ACK : SendPacket
  {
    private uint _erro;
    private List<object> _objs;

    public PROTOCOL_MESSENGER_NOTE_DELETE_ACK(uint erro, List<object> objs)
    {
      this._erro = erro;
      this._objs = objs;
    }

    public override void write()
    {
      this.writeH((short) 937);
      this.writeD(this._erro);
      this.writeC((byte) this._objs.Count);
      foreach (int num in this._objs)
        this.writeD(num);
    }
  }
}
