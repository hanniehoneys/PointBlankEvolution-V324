using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_SERVER_MESSAGE_INVITED_ACK : SendPacket
  {
    private Account sender;
    private Room room;

    public PROTOCOL_SERVER_MESSAGE_INVITED_ACK(Account sender, Room room)
    {
      this.sender = sender;
      this.room = room;
    }

    public override void write()
    {
      this.writeH((short) 2565);
      this.writeUnicode(this.sender.player_name, 66);
      this.writeD(this.room._roomId);
      this.writeQ(this.sender.player_id);
      this.writeS(this.room.password, 4);
    }
  }
}
