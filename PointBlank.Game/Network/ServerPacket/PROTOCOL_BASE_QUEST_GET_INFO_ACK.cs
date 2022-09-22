using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_QUEST_GET_INFO_ACK : SendPacket
  {
    private Account player;

    public PROTOCOL_BASE_QUEST_GET_INFO_ACK(Account player)
    {
      this.player = player;
    }

    public override void write()
    {
      this.writeH((short) 563);
      this.writeC((byte) this.player._mission.actualMission);
      this.writeC((byte) this.player._mission.actualMission);
      this.writeC((byte) this.player._mission.card1);
      this.writeC((byte) this.player._mission.card2);
      this.writeC((byte) this.player._mission.card3);
      this.writeC((byte) this.player._mission.card4);
      this.writeB(ComDiv.getCardFlags(this.player._mission.mission1, this.player._mission.list1));
      this.writeB(ComDiv.getCardFlags(this.player._mission.mission2, this.player._mission.list2));
      this.writeB(ComDiv.getCardFlags(this.player._mission.mission3, this.player._mission.list3));
      this.writeB(ComDiv.getCardFlags(this.player._mission.mission4, this.player._mission.list4));
      this.writeC((byte) this.player._mission.mission1);
      this.writeC((byte) this.player._mission.mission2);
      this.writeC((byte) this.player._mission.mission3);
      this.writeC((byte) this.player._mission.mission4);
    }
  }
}
