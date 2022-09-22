using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Clan;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_JOIN_REQUEST_REQ : ReceivePacket
  {
    private int clanId;
    private string text;
    private uint erro;

    public PROTOCOL_CS_JOIN_REQUEST_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.clanId = this.readD();
      this.text = this.readUnicode((int) this.readC() * 2);
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        ClanInvite invite = new ClanInvite()
        {
          clan_id = this.clanId,
          player_id = this._client.player_id,
          text = this.text,
          inviteDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"))
        };
        if (player.clanId > 0 || player.player_name.Length == 0)
          this.erro = 2147487836U;
        else if (ClanManager.getClan(this.clanId)._id == 0)
          this.erro = 2147483648U;
        else if (PlayerManager.getRequestCount(this.clanId) >= 100)
          this.erro = 2147487831U;
        else if (!PlayerManager.CreateInviteInDb(invite))
          this.erro = 2147487848U;
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_JOIN_REQUEST_ACK(this.erro, this.clanId));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_JOIN_REQUEST_REQ: " + ex.ToString());
      }
    }
  }
}
