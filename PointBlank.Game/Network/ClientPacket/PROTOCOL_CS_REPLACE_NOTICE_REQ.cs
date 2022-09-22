using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_REPLACE_NOTICE_REQ : ReceivePacket
  {
    private string clan_news;
    private uint erro;

    public PROTOCOL_CS_REPLACE_NOTICE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.clan_news = this.readUnicode((int) this.readC() * 2);
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player != null)
        {
          PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
          if (clan._id > 0 && clan._news != this.clan_news && (clan.owner_id == this._client.player_id || player.clanAccess >= 1 && player.clanAccess <= 2))
          {
            if (ComDiv.updateDB("clan_data", "clan_news", (object) this.clan_news, "clan_id", (object) clan._id))
              clan._news = this.clan_news;
            else
              this.erro = 2147487859U;
          }
          else
            this.erro = 2147487835U;
        }
        else
          this.erro = 2147487835U;
      }
      catch
      {
        this.erro = 2147487859U;
      }
      this._client.SendPacket((SendPacket) new PROTOCOL_CS_REPLACE_NOTICE_ACK(this.erro));
    }
  }
}
