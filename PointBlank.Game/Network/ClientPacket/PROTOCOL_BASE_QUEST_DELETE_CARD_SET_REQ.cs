using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_QUEST_DELETE_CARD_SET_REQ : ReceivePacket
  {
    private uint erro;
    private int missionIdx;

    public PROTOCOL_BASE_QUEST_DELETE_CARD_SET_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.missionIdx = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        PlayerMissions mission = player._mission;
        bool flag = false;
        if (this.missionIdx >= 3 || this.missionIdx == 0 && mission.mission1 == 0 || (this.missionIdx == 1 && mission.mission2 == 0 || this.missionIdx == 2 && mission.mission3 == 0))
          flag = true;
        if (!flag && PlayerManager.updateMissionId(player.player_id, 0, this.missionIdx))
        {
          if (ComDiv.updateDB("player_missions", "owner_id", (object) player.player_id, new string[2]
          {
            "card" + (object) (this.missionIdx + 1),
            "mission" + (object) (this.missionIdx + 1)
          }, (object) 0, (object) new byte[0]))
          {
            if (this.missionIdx == 0)
            {
              mission.mission1 = 0;
              mission.card1 = 0;
              mission.list1 = new byte[40];
              goto label_12;
            }
            else if (this.missionIdx == 1)
            {
              mission.mission2 = 0;
              mission.card2 = 0;
              mission.list2 = new byte[40];
              goto label_12;
            }
            else if (this.missionIdx == 2)
            {
              mission.mission3 = 0;
              mission.card3 = 0;
              mission.list3 = new byte[40];
              goto label_12;
            }
            else
              goto label_12;
          }
        }
        this.erro = 2147487824U;
label_12:
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_QUEST_DELETE_CARD_SET_ACK(this.erro, player));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BASE_QUEST_DELETE_CARD_SET_REQ: " + ex.ToString());
      }
    }
  }
}
