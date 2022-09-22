using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Xml;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CLAN_WAR_INVITE_ACCEPT_REQ : ReceivePacket
  {
    private int id;
    private int serverInfo;
    private int type;
    private uint erro;

    public PROTOCOL_CLAN_WAR_INVITE_ACCEPT_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.readD();
      this.id = (int) this.readH();
      this.serverInfo = (int) this.readH();
      this.type = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player == null)
          return;
        Match match1 = player._match;
        Match match2 = ChannelsXml.getChannel(this.serverInfo - this.serverInfo / 10 * 10).getMatch(this.id);
        if (match1 != null && match2 != null && player.matchSlot == match1._leader)
        {
          if (this.type == 1)
          {
            if (match1.formação != match2.formação)
              this.erro = 2147487890U;
            else if (match2.getCountPlayers() != match1.formação || match1.getCountPlayers() != match1.formação)
              this.erro = 2147487889U;
            else if (match2._state == MatchState.Play || match1._state == MatchState.Play)
            {
              this.erro = 2147487888U;
            }
            else
            {
              match1._state = MatchState.Play;
              Account leader = match2.getLeader();
              if (leader != null && leader._match != null)
              {
                leader.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_ENEMY_INFO_ACK(match1));
                leader.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_CREATE_ROOM_ACK(match1));
                match2._slots[leader.matchSlot].state = SlotMatchState.Ready;
              }
              match2._state = MatchState.Play;
            }
          }
          else
          {
            Account leader = match2.getLeader();
            if (leader != null && leader._match != null)
              leader.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_INVITE_ACCEPT_ACK(2147487891U));
          }
        }
        else
          this.erro = 2147487892U;
        this._client.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_ACCEPT_BATTLE_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info("CLAN_WAR_ACCEPT_BATTLE_REC: " + ex.ToString());
      }
    }
  }
}
