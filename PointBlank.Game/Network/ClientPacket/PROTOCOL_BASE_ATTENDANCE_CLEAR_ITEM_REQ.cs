using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Shop;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_ATTENDANCE_CLEAR_ITEM_REQ : ReceivePacket
  {
    private EventErrorEnum erro = EventErrorEnum.VISIT_EVENT_SUCCESS;
    private int eventId;
    private int type;
    private int day;

    public PROTOCOL_BASE_ATTENDANCE_CLEAR_ITEM_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.eventId = this.readD();
      this.type = (int) this.readC();
      this.day = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        if (this._client == null)
          return;
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null || player.player_name.Length == 0 || this.type > 1)
          this.erro = EventErrorEnum.VISIT_EVENT_USERFAIL;
        else if (player._event != null)
        {
          if (player._event.LastVisitSequence1 == player._event.LastVisitSequence2)
          {
            this.erro = EventErrorEnum.VISIT_EVENT_ALREADYCHECK;
          }
          else
          {
            EventVisitModel eventVisitModel = EventVisitSyncer.getEvent(this.eventId);
            if (eventVisitModel == null)
              this.erro = EventErrorEnum.VISIT_EVENT_UNKNOWN;
            else if (eventVisitModel.EventIsEnabled())
            {
              VisitItem reward = eventVisitModel.getReward(player._event.LastVisitSequence2, this.type);
              if (reward != null)
              {
                GoodItem good = ShopManager.getGood(reward.good_id);
                if (good != null)
                {
                  PlayerEvent playerEvent = player._event;
                  DateTime dateTime = DateTime.Now;
                  dateTime = dateTime.AddDays(1.0);
                  int num = int.Parse(dateTime.ToString("yyMMdd"));
                  playerEvent.NextVisitDate = num;
                  ComDiv.updateDB("player_events", "player_id", (object) player.player_id, new string[2]
                  {
                    "next_visit_date",
                    "last_visit_sequence2"
                  }, (object) player._event.NextVisitDate, (object) ++player._event.LastVisitSequence2);
                  this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, new ItemsModel(good._item._id, good._item._category, good._item._name, good._item._equip, reward.count, 0L)));
                }
                else
                  this.erro = EventErrorEnum.VISIT_EVENT_NOTENOUGH;
              }
              else
                this.erro = EventErrorEnum.VISIT_EVENT_UNKNOWN;
            }
            else
              this.erro = EventErrorEnum.VISIT_EVENT_WRONGVERSION;
          }
        }
        else
          this.erro = EventErrorEnum.VISIT_EVENT_UNKNOWN;
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_ATTENDANCE_CLEAR_ITEM_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BASE_ATTENDANCE_CLEAR_ITEM_REQ: " + ex.ToString());
      }
    }
  }
}
