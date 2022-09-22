using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_QUEST_BUY_CARD_SET_REQ : ReceivePacket
  {
    private int missionId;
    private uint erro;

    public PROTOCOL_BASE_QUEST_BUY_CARD_SET_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.missionId = (int) this.readC();
    }

    public override void run()
    {
      PointBlank.Game.Data.Model.Account player = this._client._player;
      if (player == null)
        return;
      PlayerMissions mission = player._mission;
      int missionPrice = MissionsXml.GetMissionPrice(this.missionId);
      if (player == null || missionPrice == -1 || (0 > player._gp - missionPrice || mission.mission1 == this.missionId) || (mission.mission2 == this.missionId || mission.mission3 == this.missionId))
      {
        this.erro = missionPrice == -1 ? 2147487820U : 2147487821U;
      }
      else
      {
        if (mission.mission1 == 0)
        {
          if (PlayerManager.updateMissionId(player.player_id, this.missionId, 0))
          {
            mission.mission1 = this.missionId;
            mission.list1 = new byte[40];
            mission.actualMission = 0;
            mission.card1 = 0;
          }
          else
            this.erro = 2147487820U;
        }
        else if (mission.mission2 == 0)
        {
          if (PlayerManager.updateMissionId(player.player_id, this.missionId, 1))
          {
            mission.mission2 = this.missionId;
            mission.list2 = new byte[40];
            mission.actualMission = 1;
            mission.card2 = 0;
          }
          else
            this.erro = 2147487820U;
        }
        else if (mission.mission3 == 0)
        {
          if (PlayerManager.updateMissionId(player.player_id, this.missionId, 2))
          {
            mission.mission3 = this.missionId;
            mission.list3 = new byte[40];
            mission.actualMission = 2;
            mission.card3 = 0;
          }
          else
            this.erro = 2147487820U;
        }
        else
          this.erro = 2147487822U;
        if (this.erro == 0U)
        {
          if (missionPrice == 0 || PlayerManager.updateAccountGold(player.player_id, player._gp - missionPrice))
            player._gp -= missionPrice;
          else
            this.erro = 2147487820U;
        }
      }
      this._client.SendPacket((SendPacket) new PROTOCOL_BASE_QUEST_BUY_CARD_SET_ACK(this.erro, player));
    }
  }
}
