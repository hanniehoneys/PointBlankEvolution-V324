using PointBlank.Core.Network;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_MESSENGER_NOTE_CHECK_READED_ACK : SendPacket
  {
    private List<int> msgs;

    public PROTOCOL_MESSENGER_NOTE_CHECK_READED_ACK(List<int> msgs)
    {
      this.msgs = msgs;
    }

    public override void write()
    {
      this.writeH((short) 935);
      this.writeC((byte) this.msgs.Count);
      for (int index = 0; index < this.msgs.Count; ++index)
        this.writeD(this.msgs[index]);
    }
  }
}
