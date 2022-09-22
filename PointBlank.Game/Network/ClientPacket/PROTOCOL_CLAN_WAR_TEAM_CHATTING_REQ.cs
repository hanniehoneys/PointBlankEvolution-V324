using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CLAN_WAR_TEAM_CHATTING_REQ : ReceivePacket
  {
    private ChattingType type;
    private string text;

    public PROTOCOL_CLAN_WAR_TEAM_CHATTING_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.type = (ChattingType) this.readH();
      this.text = this.readS((int) this.readH());
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player == null || player._match == null || this.type != ChattingType.Match)
          return;
        Match match = player._match;
        this.serverCommands(player, match);
        using (PROTOCOL_CLAN_WAR_TEAM_CHATTING_ACK warTeamChattingAck = new PROTOCOL_CLAN_WAR_TEAM_CHATTING_ACK(player.player_name, this.text))
          match.SendPacketToPlayers((SendPacket) warTeamChattingAck);
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CLAN_WAR_TEAM_CHATTING_REQ: " + ex.ToString());
      }
    }

    private bool serverCommands(Account player, Match match)
    {
      try
      {
        if (!player.HaveGMLevel() || !this.text.StartsWith(";") && !this.text.StartsWith("\\") && !this.text.StartsWith("."))
          return false;
        string str = this.text.Substring(1);
        if (str.StartsWith("o") && player.access >= AccessLevel.Moderator)
        {
          if (match != null)
          {
            AccountManager.getAccountDB((object) 2L, 2);
            for (int index = 0; index < match.formação; ++index)
            {
              SlotMatch slot = match._slots[index];
              if (slot._playerId == 0L)
              {
                slot._playerId = 2L;
                slot.state = SlotMatchState.Normal;
              }
            }
            using (PROTOCOL_CLAN_WAR_REGIST_MERCENARY_ACK registMercenaryAck = new PROTOCOL_CLAN_WAR_REGIST_MERCENARY_ACK(match))
              match.SendPacketToPlayers((SendPacket) registMercenaryAck);
            this.text = "Disputa preenchida. [Server]";
          }
          else
            this.text = "Falha ao encher a disputa. [Server]";
        }
        else if (str.StartsWith("gg"))
        {
          match._state = MatchState.Play;
          match._slots[player.matchSlot].state = SlotMatchState.Ready;
          this._client.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_ENEMY_INFO_ACK(match));
          this._client.SendPacket((SendPacket) new PROTOCOL_CLAN_WAR_CREATE_ROOM_ACK(match));
        }
        else
          this.text = "Falha ao encontrar o comando digitado. [Servidor]";
        return true;
      }
      catch (OverflowException ex)
      {
        ex.ToString();
        return true;
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
        return true;
      }
    }
  }
}
