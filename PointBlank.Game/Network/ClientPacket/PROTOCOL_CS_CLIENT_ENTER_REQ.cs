using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_CLIENT_ENTER_REQ : ReceivePacket
  {
    private int id;

    public PROTOCOL_CS_CLIENT_ENTER_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        Room room = player._room;
        if (room != null)
        {
          room.changeSlotState(player._slotId, SlotState.CLAN, false);
          room.StopCountDown(player._slotId);
          room.updateSlotsInfo();
        }
        PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
        if (player.clanId == 0 && player.player_name.Length > 0)
          this.id = PlayerManager.getRequestClanId(player.player_id);
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_CLIENT_ENTER_ACK(this.id > 0 ? this.id : clan._id, player.clanAccess));
        if (clan._id <= 0 || this.id != 0)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_DETAIL_INFO_ACK(0, clan));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_CLIENT_ENTER_REQ: " + ex.ToString());
      }
    }
  }
}
