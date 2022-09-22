using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Xml;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CLAN_WAR_JOIN_TEAM_REQ : ReceivePacket
  {
    private int matchId;
    private int serverInfo;
    private int type;
    private uint erro;

    public PROTOCOL_CLAN_WAR_JOIN_TEAM_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.matchId = (int) this.readH();
      this.serverInfo = (int) this.readH();
      this.type = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (this.type >= 2 || player == null || (player._match != null || player._room != null))
        {
          this._client.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_JOIN_TEAM_ACK(2147483648U, (Match) null));
        }
        else
        {
          int num = this.serverInfo - this.serverInfo / 10 * 10;
          Channel channel = ChannelsXml.getChannel(this.type == 0 ? num : player.channelId);
          if (channel != null)
          {
            if (player.clanId == 0)
            {
              this._client.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_JOIN_TEAM_ACK(2147487835U, (Match) null));
            }
            else
            {
              Match mt = this.type == 1 ? channel.getMatch(this.matchId, player.clanId) : channel.getMatch(this.matchId);
              if (mt != null)
                this.JoinPlayer(player, mt);
              else
                this._client.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_JOIN_TEAM_ACK(2147483648U, (Match) null));
            }
          }
          else
            this._client.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_JOIN_TEAM_ACK(2147483648U, (Match) null));
        }
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }

    private void JoinPlayer(Account p, Match mt)
    {
      if (!mt.addPlayer(p))
        this.erro = 2147483648U;
      this._client.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_JOIN_TEAM_ACK(this.erro, mt));
      if (this.erro != 0U)
        return;
      using (PROTOCOL_CLAN_WAR_REGIST_MERCENARY_ACK registMercenaryAck = new PROTOCOL_CLAN_WAR_REGIST_MERCENARY_ACK(mt))
        mt.SendPacketToPlayers((SendPacket) registMercenaryAck);
    }
  }
}
