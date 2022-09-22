using PointBlank.Core.Models.Servers;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Configs;

namespace PointBlank.Game.Data.Sync.Server
{
  public class SendClanInfo
  {
    public static void Load(PointBlank.Game.Data.Model.Account pl, PointBlank.Game.Data.Model.Account member, int type)
    {
      if (pl == null)
        return;
      GameServerModel server = GameSync.GetServer(pl._status);
      if (server == null)
        return;
      using (SendGPacket sendGpacket = new SendGPacket())
      {
        sendGpacket.writeH((short) 16);
        sendGpacket.writeQ(pl.player_id);
        sendGpacket.writeC((byte) type);
        switch (type)
        {
          case 1:
            sendGpacket.writeQ(member.player_id);
            sendGpacket.writeC((byte) (member.player_name.Length + 1));
            sendGpacket.writeS(member.player_name, member.player_name.Length + 1);
            sendGpacket.writeB(member._status.buffer);
            sendGpacket.writeC((byte) member._rank);
            break;
          case 2:
            sendGpacket.writeQ(member.player_id);
            break;
          case 3:
            sendGpacket.writeD(pl.clanId);
            sendGpacket.writeC((byte) pl.clanAccess);
            break;
        }
        GameSync.SendPacket(sendGpacket.mstream.ToArray(), server.Connection);
      }
    }

    public static void Update(PointBlank.Core.Models.Account.Clan.Clan clan, int type)
    {
      for (int index = 0; index < ServersXml._servers.Count; ++index)
      {
        GameServerModel server = ServersXml._servers[index];
        if (server._id != 0 && server._id != GameConfig.serverId)
        {
          using (SendGPacket sendGpacket = new SendGPacket())
          {
            sendGpacket.writeH((short) 22);
            sendGpacket.writeC((byte) type);
            switch (type)
            {
              case 0:
                sendGpacket.writeQ(clan.owner_id);
                break;
              case 1:
                sendGpacket.writeC((byte) (clan._name.Length + 1));
                sendGpacket.writeS(clan._name, clan._name.Length + 1);
                break;
              case 2:
                sendGpacket.writeC((byte) clan._name_color);
                break;
            }
            GameSync.SendPacket(sendGpacket.mstream.ToArray(), server.Connection);
          }
        }
      }
    }

    public static void Load(PointBlank.Core.Models.Account.Clan.Clan clan, int type)
    {
      for (int index = 0; index < ServersXml._servers.Count; ++index)
      {
        GameServerModel server = ServersXml._servers[index];
        if (server._id != 0 && server._id != GameConfig.serverId)
        {
          using (SendGPacket sendGpacket = new SendGPacket())
          {
            sendGpacket.writeH((short) 21);
            sendGpacket.writeC((byte) type);
            sendGpacket.writeD(clan._id);
            if (type == 0)
            {
              sendGpacket.writeQ(clan.owner_id);
              sendGpacket.writeD(clan.creationDate);
              sendGpacket.writeC((byte) (clan._name.Length + 1));
              sendGpacket.writeS(clan._name, clan._name.Length + 1);
              sendGpacket.writeC((byte) (clan._info.Length + 1));
              sendGpacket.writeS(clan._info, clan._info.Length + 1);
            }
            GameSync.SendPacket(sendGpacket.mstream.ToArray(), server.Connection);
          }
        }
      }
    }
  }
}
