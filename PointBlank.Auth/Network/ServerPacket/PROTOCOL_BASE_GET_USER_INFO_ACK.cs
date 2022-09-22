using PointBlank.Auth.Data.Managers;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Servers;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using System;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_GET_USER_INFO_ACK : SendPacket
  {
    private PointBlank.Auth.Data.Model.Account Player;
    private PointBlank.Core.Models.Account.Clan.Clan Clan;
    private uint Error;

    public PROTOCOL_BASE_GET_USER_INFO_ACK(PointBlank.Auth.Data.Model.Account Player)
    {
      this.Player = Player;
      if (Player != null)
        this.Clan = ClanManager.getClanDB((object) Player.clan_id, 1);
      else
        this.Error = 2147483648U;
    }

    private void CheckGameEvents(EventVisitModel evVisit)
    {
      long num = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
      PlayerEvent pE = this.Player._event;
      if (pE != null)
      {
        QuestModel runningEvent1 = EventQuestSyncer.getRunningEvent();
        if (runningEvent1 != null)
        {
          long lastQuestDate = (long) pE.LastQuestDate;
          long lastQuestFinish = (long) pE.LastQuestFinish;
          if (pE.LastQuestDate < runningEvent1.startDate)
          {
            pE.LastQuestDate = 0U;
            pE.LastQuestFinish = 0;
          }
          if (pE.LastQuestFinish == 0)
          {
            this.Player._mission.mission4 = 13;
            if (pE.LastQuestDate == 0U)
              pE.LastQuestDate = (uint) num;
          }
          if ((long) pE.LastQuestDate != lastQuestDate || (long) pE.LastQuestFinish != lastQuestFinish)
            EventQuestSyncer.ResetPlayerEvent(this.Player.player_id, pE);
        }
        EventLoginModel runningEvent2 = EventLoginSyncer.getRunningEvent();
        if (runningEvent2 != null && (runningEvent2.startDate >= pE.LastLoginDate || pE.LastLoginDate >= runningEvent2.endDate))
        {
          PlayerManager.tryCreateItem(new ItemsModel(runningEvent2._rewardId, runningEvent2._category, "Login Event", 1, runningEvent2._count, 0L), this.Player._inventory, this.Player.player_id);
          ComDiv.updateDB("player_events", "last_login_date", (object) num, "player_id", (object) this.Player.player_id);
        }
        if (evVisit != null && pE.LastVisitEventId != evVisit.id)
        {
          pE.LastVisitEventId = evVisit.id;
          pE.LastVisitSequence1 = 0;
          pE.LastVisitSequence2 = 0;
          pE.NextVisitDate = 0;
          EventVisitSyncer.ResetPlayerEvent(this.Player.player_id, evVisit.id);
        }
      }
      ComDiv.updateDB("accounts", "last_login", (object) num, "player_id", (object) this.Player.player_id);
    }

    public override void write()
    {
      ServerConfig config = AuthManager.Config;
      EventVisitModel runningEvent = EventVisitSyncer.getRunningEvent();
      this.writeH((short) 525);
      this.writeH((short) 0);
      this.writeD(this.Error);
      if (this.Error != 0U)
        return;
      this.writeC((byte) this.Player.Characters.Count);
      this.writeH((short) 210);
      this.writeC((byte) QuickStartXml.QucikStarts.Count);
      for (int index = 0; index < QuickStartXml.QucikStarts.Count; ++index)
      {
        QuickStart qucikStart = QuickStartXml.QucikStarts[index];
        this.writeC((byte) qucikStart.MapId);
        this.writeC((byte) qucikStart.Rule);
        this.writeC((byte) qucikStart.StageOptions);
        this.writeC((byte) qucikStart.Type);
      }
      this.writeB(new byte[33]);
      this.writeC((byte) 4);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeD(this.Player._titles.Slots);
      this.writeC((byte) 3);
      this.writeC((byte) this.Player._titles.Equiped1);
      this.writeC((byte) this.Player._titles.Equiped2);
      this.writeC((byte) this.Player._titles.Equiped3);
      this.writeQ(this.Player._titles.Flags);
      this.writeC((byte) 160);
      this.writeB(this.Player._mission.list1);
      this.writeB(this.Player._mission.list2);
      this.writeB(this.Player._mission.list3);
      this.writeB(this.Player._mission.list4);
      this.writeC((byte) this.Player._mission.actualMission);
      this.writeC((byte) this.Player._mission.card1);
      this.writeC((byte) this.Player._mission.card2);
      this.writeC((byte) this.Player._mission.card3);
      this.writeC((byte) this.Player._mission.card4);
      this.writeB(ComDiv.getCardFlags(this.Player._mission.mission1, this.Player._mission.list1));
      this.writeB(ComDiv.getCardFlags(this.Player._mission.mission2, this.Player._mission.list2));
      this.writeB(ComDiv.getCardFlags(this.Player._mission.mission3, this.Player._mission.list3));
      this.writeB(ComDiv.getCardFlags(this.Player._mission.mission4, this.Player._mission.list4));
      this.writeC((byte) this.Player._mission.mission1);
      this.writeC((byte) this.Player._mission.mission2);
      this.writeC((byte) this.Player._mission.mission3);
      this.writeC((byte) this.Player._mission.mission4);
      this.writeD(this.Player.blue_order);
      this.writeD(this.Player.medal);
      this.writeD(this.Player.insignia);
      this.writeD(this.Player.brooch);
      this.writeD(0);
      this.writeC((byte) 0);
      this.writeD(0);
      this.writeC((byte) 2);
      this.WriteDormantEvent();
      this.WriteVisitEvent(runningEvent);
      this.writeC((byte) 2);
      this.writeD(0);
      this.writeC((byte) 0);
      this.writeD(0);
      this.writeD(0);
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeIP("127.0.0.1");
      this.writeD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
      if (this.Player.Characters.Count == 0)
      {
        this.writeC((byte) 0);
        this.writeC((byte) 1);
      }
      else
      {
        this.writeC((byte) this.Player.getCharacter(this.Player._equip._red).Slot);
        this.writeC((byte) this.Player.getCharacter(this.Player._equip._blue).Slot);
      }
      this.writeD(this.Player._inventory.getItem(this.Player._equip._dino)._id);
      this.writeD((uint) this.Player._inventory.getItem(this.Player._equip._dino)._objId);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeH((short) 0);
      this.writeC((byte) this.Player.name_color);
      this.writeD(this.Player._bonus.fakeRank);
      this.writeD(this.Player._bonus.fakeRank);
      this.writeUnicode(this.Player._bonus.fakeNick, 66);
      this.writeH((short) this.Player._bonus.sightColor);
      this.writeH((short) this.Player._bonus.muzzle);
      this.writeD(this.Player._statistic.fights);
      this.writeD(this.Player._statistic.fights_win);
      this.writeD(this.Player._statistic.fights_lost);
      this.writeD(this.Player._statistic.fights_draw);
      this.writeD(this.Player._statistic.kills_count);
      this.writeD(this.Player._statistic.headshots_count);
      this.writeD(this.Player._statistic.deaths_count);
      this.writeD(this.Player._statistic.totalfights_count);
      this.writeD(this.Player._statistic.totalkills_count);
      this.writeD(this.Player._statistic.escapes);
      this.writeD(this.Player._statistic.assist);
      this.writeD(this.Player._statistic.fights);
      this.writeD(this.Player._statistic.fights_win);
      this.writeD(this.Player._statistic.fights_lost);
      this.writeD(this.Player._statistic.fights_draw);
      this.writeD(this.Player._statistic.kills_count);
      this.writeD(this.Player._statistic.headshots_count);
      this.writeD(this.Player._statistic.deaths_count);
      this.writeD(this.Player._statistic.totalfights_count);
      this.writeD(this.Player._statistic.totalkills_count);
      this.writeD(this.Player._statistic.escapes);
      this.writeD(this.Player._statistic.assist);
      this.writeUnicode(this.Player.player_name, 66);
      this.writeD(this.Player._rank);
      this.writeD(this.Player._rank);
      this.writeD(this.Player._gp);
      this.writeD(this.Player._exp);
      this.writeD(0);
      this.writeC((byte) 0);
      this.writeD(0);
      this.writeQ(0L);
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeD(this.Player._money);
      this.writeD(this.Clan._id);
      this.writeD(this.Player.clanAccess);
      this.writeQ(this.Player.Status());
      this.writeC((byte) this.Player.pc_cafe);
      this.writeC((byte) this.Player.tourneyLevel);
      this.writeUnicode(this.Clan._name, 34);
      this.writeC((byte) this.Clan._rank);
      this.writeC((byte) this.Clan.getClanUnit());
      this.writeD(this.Clan._logo);
      this.writeC((byte) this.Clan._name_color);
      this.writeC((byte) this.Clan.effect);
      this.writeC((byte) this.Player.age);
    }

    private void WriteDormantEvent()
    {
      this.writeB(new byte[375]);
    }

    private void WriteVisitEvent(EventVisitModel ev)
    {
      PlayerEvent playerEvent = this.Player._event;
      if (ev != null && (playerEvent.LastVisitSequence1 < ev.checks && playerEvent.NextVisitDate <= int.Parse(DateTime.Now.ToString("yyMMdd")) || playerEvent.LastVisitSequence2 < ev.checks && playerEvent.LastVisitSequence2 != playerEvent.LastVisitSequence1))
      {
        this.writeUnicode(ev.title, 70);
        this.writeC((byte) playerEvent.LastVisitSequence1);
        this.writeC((byte) ev.checks);
        this.writeD(ev.id);
        this.writeD(ev.startDate);
        this.writeD(ev.endDate);
        this.writeB(new byte[12]);
        for (int index = 0; index < 32; ++index)
        {
          VisitBox visitBox = ev.box[index];
          this.writeC((byte) visitBox.RewardCount);
          this.writeD(visitBox.reward1.good_id);
          this.writeD(visitBox.reward2.good_id);
        }
      }
      else
        this.writeB(new byte[375]);
    }
  }
}
