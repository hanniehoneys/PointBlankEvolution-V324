using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Xml;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CLAN_WAR_MATCH_PROPOSE_REQ : ReceivePacket
  {
    private int id;
    private int serverInfo;
    private uint erro;

    public PROTOCOL_CLAN_WAR_MATCH_PROPOSE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.id = (int) this.readH();
      this.serverInfo = (int) this.readH();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player != null && player._match != null && (player.matchSlot == player._match._leader && player._match._state == MatchState.Ready))
        {
          Match match = ChannelsXml.getChannel(this.serverInfo - this.serverInfo / 10 * 10).getMatch(this.id);
          if (match != null)
          {
            Account leader = match.getLeader();
            if (leader != null && leader._connection != null && leader._isOnline)
              leader.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_CHANGE_MAX_PER_ACK(player._match, player));
            else
              this.erro = 2147483648U;
          }
          else
            this.erro = 2147483648U;
        }
        else
          this.erro = 2147483648U;
        this._client.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_MATCH_PROPOSE_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CLAN_WAR_MATCH_PROPOSE_REQ: " + ex.ToString());
      }
    }
  }
}
