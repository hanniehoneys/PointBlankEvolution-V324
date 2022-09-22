using PointBlank.Core.Models.Map;
using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_MAP_RULELIST_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 669);
      this.writeH((short) 0);
      this.writeH((short) MapModel.Rules.Count);
      for (int index = 0; index < MapModel.Rules.Count; ++index)
      {
        MapRule rule = MapModel.Rules[index];
        this.writeD(rule.Id);
        this.writeC((byte) 0);
        this.writeC((byte) rule.Rule);
        this.writeC((byte) rule.StageOptions);
        this.writeC((byte) rule.Conditions);
        this.writeC((byte) 0);
        this.writeS(rule.Name, 67);
      }
    }
  }
}
