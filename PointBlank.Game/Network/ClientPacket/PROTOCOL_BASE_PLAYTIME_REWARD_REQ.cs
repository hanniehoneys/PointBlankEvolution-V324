using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Shop;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_PLAYTIME_REWARD_REQ : ReceivePacket
  {
    private int goodId;

    public PROTOCOL_BASE_PLAYTIME_REWARD_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.goodId = this.readD();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        PlayerEvent playerEvent = player._event;
        GoodItem good = ShopManager.getGood(this.goodId);
        if (good == null || playerEvent == null)
          return;
        PlayTimeModel runningEvent = EventPlayTimeSyncer.getRunningEvent();
        if (runningEvent == null)
          return;
        long rewardCount = runningEvent.GetRewardCount(this.goodId);
        if (playerEvent.LastPlaytimeFinish != 1 || rewardCount <= 0L || !ComDiv.updateDB("player_events", "last_playtime_finish", (object) 2, "player_id", (object) this._client.player_id))
          return;
        playerEvent.LastPlaytimeFinish = 2;
        this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, new ItemsModel(good._item._id, good._item._category, "PlayTime Reward", good._item._equip, rewardCount, 0L)));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BASE_PLAYTIME_REWARD_REQ: " + ex.ToString());
      }
    }
  }
}
