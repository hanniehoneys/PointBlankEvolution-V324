using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_DETAIL_INFO_REQ : ReceivePacket
  {
    private int clanId;
    private int unk;

    public PROTOCOL_CS_DETAIL_INFO_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.clanId = this.readD();
      this.unk = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        player.FindClanId = this.clanId;
        PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(this.clanId);
        if (clan._id <= 0)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_DETAIL_INFO_ACK(this.unk, clan));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_DETAIL_INFO_REQ: " + ex.ToString());
      }
    }
  }
}
