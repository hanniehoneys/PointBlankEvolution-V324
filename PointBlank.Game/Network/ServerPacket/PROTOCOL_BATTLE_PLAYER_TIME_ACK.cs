using PointBlank.Core.Managers.Events;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_PLAYER_TIME_ACK : SendPacket
  {
    private int _type;
    private PlayTimeModel ev;

    public PROTOCOL_BATTLE_PLAYER_TIME_ACK(int type, PlayTimeModel eventPt)
    {
      this._type = type;
      this.ev = eventPt;
    }

    public override void write()
    {
      this.writeH((short) 3911);
      this.writeC((byte) this._type);
      this.writeS(this.ev._title, 30);
      this.writeD(this.ev._goodReward1);
      this.writeD(this.ev._goodReward2);
      this.writeQ(this.ev._time);
    }
  }
}
