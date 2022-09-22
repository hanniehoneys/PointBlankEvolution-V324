using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Model;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_GET_CHANNELLIST_ACK : SendPacket
  {
    private List<Channel> Channels;

    public PROTOCOL_BASE_GET_CHANNELLIST_ACK(List<Channel> Channels)
    {
      this.Channels = Channels;
    }

    public override void write()
    {
      this.writeH((short) 541);
      this.writeH((short) 0);
      this.writeC((byte) 0);
      this.writeC((byte) this.Channels.Count);
      for (int index = 0; index < this.Channels.Count; ++index)
        this.writeH((ushort) this.Channels[index]._players.Count);
      this.writeH((ushort) GameConfig.maxChannelPlayers);
      this.writeC((byte) this.Channels.Count);
    }
  }
}
