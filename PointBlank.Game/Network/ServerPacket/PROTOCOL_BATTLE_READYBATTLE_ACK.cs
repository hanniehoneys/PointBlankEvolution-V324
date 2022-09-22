using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_READYBATTLE_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_BATTLE_READYBATTLE_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 4100);
      this.writeC((byte) 0);
      this.writeH((short) 0);
      this.writeD(this._erro);
    }
  }
}
