using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_CHATTING_ACK : SendPacket
  {
    private string msg;
    private int type;
    private int slotId;
    private bool GMColor;

    public PROTOCOL_ROOM_CHATTING_ACK(int chat_type, int slotId, bool GM, string message)
    {
      this.type = chat_type;
      this.slotId = slotId;
      this.GMColor = GM;
      this.msg = message;
    }

    public override void write()
    {
      this.writeH((short) 3862);
      this.writeH((short) this.type);
      this.writeD(this.slotId);
      this.writeC(this.GMColor);
      this.writeD(this.msg.Length + 1);
      this.writeUnicode(this.msg, true);
    }
  }
}
