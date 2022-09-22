using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_CLAN_LIST_DETAIL_INFO_REQ : ReceivePacket
  {
    private int ClanId;
    private int Unk;

    public PROTOCOL_CS_CLAN_LIST_DETAIL_INFO_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.ClanId = this.readD();
      this.Unk = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        player.FindClanId = this.ClanId;
        PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(this.ClanId);
        if (clan._id <= 0)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_CLAN_LIST_DETAIL_INFO_ACK(this.Unk, clan));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_CLAN_LIST_DETAIL_INFO_REQ: " + ex.ToString());
      }
    }
  }
}
