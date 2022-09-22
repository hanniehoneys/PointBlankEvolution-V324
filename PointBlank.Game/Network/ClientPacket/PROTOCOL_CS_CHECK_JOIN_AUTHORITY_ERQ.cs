using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_CHECK_JOIN_AUTHORITY_ERQ : ReceivePacket
  {
    private int clanId;
    private uint erro;

    public PROTOCOL_CS_CHECK_JOIN_AUTHORITY_ERQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.clanId = this.readD();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(this.clanId);
        if (clan._id == 0)
          this.erro = 2147483648U;
        else if (clan.limite_rank > player._rank)
          this.erro = 2147487867U;
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_CHECK_JOIN_AUTHORITY_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_CHECK_JOIN_AUTHORITY_ERQ: " + ex.ToString());
      }
    }
  }
}
