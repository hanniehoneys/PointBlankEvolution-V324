using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_CHATTING_REQ : ReceivePacket
  {
    private ChattingType type;
    private string text;

    public PROTOCOL_CS_CHATTING_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.type = (ChattingType) this.readH();
      this.text = this.readUnicode((int) this.readH() * 2);
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player == null || this.text.Length > 60 || this.type != ChattingType.Clan)
          return;
        using (PROTOCOL_CS_CHATTING_ACK protocolCsChattingAck = new PROTOCOL_CS_CHATTING_ACK(this.text, player))
          ClanManager.SendPacket((SendPacket) protocolCsChattingAck, player.clanId, -1L, true, true);
      }
      catch
      {
      }
    }
  }
}
