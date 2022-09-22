using PointBlank.Core;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_ATTENDANCE_REQ : ReceivePacket
  {
    private EventErrorEnum erro = EventErrorEnum.VISIT_EVENT_SUCCESS;
    private int eventId;
    private int day;
    private EventVisitModel eventv;

    public PROTOCOL_BASE_ATTENDANCE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.eventId = this.readD();
      this.day = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        if (this._client == null)
          return;
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null || string.IsNullOrEmpty(player.player_name))
          this.erro = EventErrorEnum.VISIT_EVENT_USERFAIL;
        else if (player._event != null)
        {
          DateTime dateTime = DateTime.Now;
          int num1 = int.Parse(dateTime.ToString("yyMMdd"));
          if (player._event.NextVisitDate <= num1)
          {
            this.eventv = EventVisitSyncer.getEvent(this.eventId);
            if (this.eventv == null)
              this.erro = EventErrorEnum.VISIT_EVENT_UNKNOWN;
            else if (this.eventv.EventIsEnabled())
            {
              PlayerEvent playerEvent = player._event;
              dateTime = DateTime.Now;
              dateTime = dateTime.AddDays(1.0);
              int num2 = int.Parse(dateTime.ToString("yyMMdd"));
              playerEvent.NextVisitDate = num2;
              ComDiv.updateDB("player_events", "player_id", (object) player.player_id, new string[2]
              {
                "next_visit_date",
                "last_visit_sequence1"
              }, (object) player._event.NextVisitDate, (object) ++player._event.LastVisitSequence1);
              bool flag = false;
              try
              {
                flag = this.eventv.box[player._event.LastVisitSequence2].reward1.IsReward;
              }
              catch
              {
              }
              if (!flag)
                ComDiv.updateDB("player_events", "last_visit_sequence2", (object) ++player._event.LastVisitSequence2, "player_id", (object) player.player_id);
            }
            else
              this.erro = EventErrorEnum.VISIT_EVENT_WRONGVERSION;
          }
          else
            this.erro = EventErrorEnum.VISIT_EVENT_ALREADYCHECK;
        }
        else
          this.erro = EventErrorEnum.VISIT_EVENT_UNKNOWN;
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_ATTENDANCE_ACK(this.erro, this.eventv, player._event));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BASE_ATTENDANCE_REQ: " + ex.ToString());
      }
    }
  }
}
