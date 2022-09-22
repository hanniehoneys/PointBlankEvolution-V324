using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Clan;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_REQUEST_LIST_REQ : ReceivePacket
  {
    private int page;

    public PROTOCOL_CS_REQUEST_LIST_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.page = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        if (player.clanId == 0)
        {
          this._client.SendPacket((SendPacket) new PROTOCOL_CS_REQUEST_LIST_ACK(-1));
        }
        else
        {
          List<ClanInvite> clanRequestList = PlayerManager.getClanRequestList(player.clanId);
          using (SendGPacket p = new SendGPacket())
          {
            int count = 0;
            for (int index = this.page * 13; index < clanRequestList.Count; ++index)
            {
              this.WriteData(clanRequestList[index], p);
              if (++count == 13)
                break;
            }
            this._client.SendPacket((SendPacket) new PROTOCOL_CS_REQUEST_LIST_ACK(0, count, this.page, p.mstream.ToArray()));
          }
        }
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_REQUEST_LIST_REQ: " + ex.ToString());
      }
    }

    private void WriteData(ClanInvite invite, SendGPacket p)
    {
      p.writeQ(invite.player_id);
      PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(invite.player_id, 0);
      if (account != null)
      {
        p.writeUnicode(account.player_name, 66);
        p.writeC((byte) account._rank);
        p.writeC((byte) account.name_color);
        p.writeD(invite.inviteDate);
        p.writeD(account._statistic.kills_count);
        p.writeD(account._statistic.deaths_count);
        p.writeD(account._statistic.fights);
        p.writeD(account._statistic.fights_win);
        p.writeD(account._statistic.fights_lost);
        p.writeUnicode(invite.text, true);
      }
      p.writeD(invite.inviteDate);
    }
  }
}
