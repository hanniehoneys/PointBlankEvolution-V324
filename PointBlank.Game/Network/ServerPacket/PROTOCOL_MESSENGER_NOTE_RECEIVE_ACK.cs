using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK : SendPacket
  {
    private Message msg;

    public PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(Message msg)
    {
      this.msg = msg;
    }

    public override void write()
    {
      this.writeH((short) 939);
      this.writeD(this.msg.object_id);
      this.writeQ(this.msg.sender_id);
      this.writeC((byte) this.msg.type);
      this.writeC((byte) this.msg.state);
      this.writeC((byte) this.msg.DaysRemaining);
      this.writeD(this.msg.clanId);
      this.writeC((byte) (this.msg.sender_name.Length + 1));
      this.writeC(this.msg.type == 5 || this.msg.type == 4 && this.msg.cB != NoteMessageClan.None ? (byte) 0 : (byte) (this.msg.text.Length + 1));
      this.writeUnicode(this.msg.sender_name, true);
      if (this.msg.type == 5 || this.msg.type == 4)
      {
        if (this.msg.cB >= NoteMessageClan.JoinAccept && this.msg.cB <= NoteMessageClan.Secession)
        {
          this.writeH((short) (this.msg.text.Length + 1));
          this.writeH((short) this.msg.cB);
          this.writeUnicode(this.msg.text, false);
        }
        else if (this.msg.cB == NoteMessageClan.None)
        {
          this.writeUnicode(this.msg.text, true);
        }
        else
        {
          this.writeH((short) 3);
          this.writeD((int) this.msg.cB);
          if (this.msg.cB == NoteMessageClan.Master && this.msg.cB == NoteMessageClan.Staff && this.msg.cB == NoteMessageClan.Regular)
            return;
          this.writeH((short) 0);
        }
      }
      else
        this.writeUnicode(this.msg.text, true);
    }
  }
}
