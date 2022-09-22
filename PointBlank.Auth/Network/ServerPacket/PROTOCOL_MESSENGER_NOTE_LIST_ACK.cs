using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using System.Collections.Generic;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_MESSENGER_NOTE_LIST_ACK : SendPacket
  {
    private int pageIdx;
    private List<Message> msgs;

    public PROTOCOL_MESSENGER_NOTE_LIST_ACK(int pageIdx, List<Message> msgs)
    {
      this.pageIdx = pageIdx;
      this.msgs = new List<Message>();
      int num = 0;
      for (int index = pageIdx * 25; index < msgs.Count; ++index)
      {
        this.msgs.Add(msgs[index]);
        if (++num == 25)
          break;
      }
    }

    public override void write()
    {
      this.writeH((short) 933);
      this.writeC((byte) this.pageIdx);
      this.writeC((byte) this.msgs.Count);
      for (int index = 0; index < this.msgs.Count; ++index)
      {
        Message msg = this.msgs[index];
        this.writeD(msg.object_id);
        this.writeQ(msg.sender_id);
        this.writeC((byte) msg.type);
        this.writeC((byte) msg.state);
        this.writeC((byte) msg.DaysRemaining);
        this.writeD(msg.clanId);
      }
      for (int index = 0; index < this.msgs.Count; ++index)
      {
        Message msg = this.msgs[index];
        this.writeC((byte) (msg.sender_name.Length + 1));
        this.writeC(msg.type == 5 || msg.type == 4 && msg.cB != NoteMessageClan.None ? (byte) 0 : (byte) (msg.text.Length + 1));
        this.writeUnicode(msg.sender_name, true);
        if (msg.type == 5 || msg.type == 4)
        {
          if (msg.cB >= NoteMessageClan.JoinAccept && msg.cB <= NoteMessageClan.Secession)
          {
            this.writeH((short) (msg.text.Length + 1));
            this.writeH((short) msg.cB);
            this.writeUnicode(msg.text, false);
          }
          else if (msg.cB == NoteMessageClan.None)
          {
            this.writeUnicode(msg.text, true);
          }
          else
          {
            this.writeH((short) 3);
            this.writeD((int) msg.cB);
            if (msg.cB != NoteMessageClan.Master || msg.cB != NoteMessageClan.Staff || msg.cB != NoteMessageClan.Regular)
              this.writeH((short) 0);
          }
        }
        else
          this.writeUnicode(msg.text, true);
      }
    }
  }
}
