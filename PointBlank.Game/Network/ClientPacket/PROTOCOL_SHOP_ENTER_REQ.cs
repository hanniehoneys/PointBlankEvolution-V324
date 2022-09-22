using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_SHOP_ENTER_REQ : ReceivePacket
  {
    private string md5;

    public PROTOCOL_SHOP_ENTER_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.md5 = this.readS(32);
    }

    public override void run()
    {
      try
      {
        if (this._client == null)
          return;
        PointBlank.Game.Data.Model.Account player = this._client._player;
        Room room = player == null ? (Room) null : player._room;
        if (room != null)
        {
          room.changeSlotState(player._slotId, SlotState.SHOP, false);
          room.StopCountDown(player._slotId);
          room.updateSlotsInfo();
        }
        player._topups = PlayerManager.getPlayerTopups(player.player_id);
        if (player._topups.Count > 0)
        {
          for (int index = 0; index < player._topups.Count; ++index)
          {
            PlayerItemTopup topup = player._topups[index];
            if (topup.ItemId != 0)
            {
              this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, new ItemsModel(topup.ItemId, topup.ItemName, topup.Equip, topup.Count, 0L)));
              PlayerManager.DeletePlayerTopup(topup.ObjectId, player.player_id);
            }
          }
        }
        this._client.SendPacket((SendPacket) new PROTOCOL_SHOP_ENTER_ACK());
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }
  }
}
