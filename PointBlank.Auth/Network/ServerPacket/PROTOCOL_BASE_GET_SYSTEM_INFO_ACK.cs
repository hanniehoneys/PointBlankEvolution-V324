using PointBlank.Auth.Data.Xml;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Servers;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using System.Collections.Generic;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_GET_SYSTEM_INFO_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 523);
      this.writeH((short) 0);
      this.writeC((byte) 0);
      this.writeC((byte) 5);
      this.writeC((byte) 10);
      this.writeC((byte) 32);
      this.writeC((byte) 4);
      this.writeC((byte) 0);
      this.writeC((byte) 1);
      this.writeC((byte) 2);
      this.writeC((byte) 5);
      this.writeC((byte) 3);
      this.writeC((byte) 6);
      this.writeB(new byte[25]);
      this.writeC((byte) 7);
      this.writeB(new byte[229]);
      this.writeD(171718655);
      this.writeC((byte) 3);
      this.writeD(600);
      this.writeD(2400);
      this.writeD(6000);
      this.writeC((byte) 0);
      this.writeH((ushort) MissionsXml._missionPage1);
      this.writeH((ushort) MissionsXml._missionPage2);
      this.writeH((short) 29890);
      this.writeC((byte) ServersXml._servers.Count);
      for (int ServerId = 0; ServerId < ServersXml._servers.Count; ++ServerId)
      {
        GameServerModel server = ServersXml._servers[ServerId];
        this.writeD(server._state);
        this.writeIP(server.Connection.Address);
        this.writeH(server._port);
        this.writeC((byte) server._type);
        this.writeH((ushort) server._maxPlayers);
        this.writeD(server._LastCount);
        if (ServerId == 0)
        {
          for (int index = 0; index < 10; ++index)
            this.writeC((byte) 1);
        }
        else
        {
          for (int index = 0; index < ChannelsXml.getChannels(ServerId).Count; ++index)
            this.writeC((byte) ChannelsXml._channels[index]._type);
        }
      }
      this.writeH((ushort) AuthManager.Config.ExitURL.Length);
      this.writeS(AuthManager.Config.ExitURL, AuthManager.Config.ExitURL.Length);
      this.writeC((byte) 51);
      for (int rank = 0; rank < 52; ++rank)
      {
        List<ItemsModel> awards = RankXml.getAwards(rank);
        this.writeC((byte) rank);
        for (int index = 0; index < awards.Count; ++index)
        {
          ItemsModel itemsModel = awards[index];
          if (ShopManager.getItemId(itemsModel._id) == null)
            this.writeD(0);
          else
            this.writeD(ShopManager.getItemId(itemsModel._id).id);
        }
        for (int count = awards.Count; 4 - count > 0; ++count)
          this.writeD(0);
      }
      this.writeC((byte) 1);
    }
  }
}
