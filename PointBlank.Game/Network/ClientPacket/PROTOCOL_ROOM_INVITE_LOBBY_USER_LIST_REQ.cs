using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_ROOM_INVITE_LOBBY_USER_LIST_REQ : ReceivePacket
  {
    private int count;
    private uint erro;

    public PROTOCOL_ROOM_INVITE_LOBBY_USER_LIST_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.count = this.readD();
    }

    public override void run()
    {
      Account player = this._client._player;
      if (player != null && player._room != null)
      {
        Channel channel = player.getChannel();
        if (channel != null)
        {
          using (PROTOCOL_SERVER_MESSAGE_INVITED_ACK messageInvitedAck = new PROTOCOL_SERVER_MESSAGE_INVITED_ACK(player, player._room))
          {
            byte[] completeBytes = messageInvitedAck.GetCompleteBytes(nameof (PROTOCOL_ROOM_INVITE_LOBBY_USER_LIST_REQ));
            for (int index = 0; index < this.count; ++index)
            {
              try
              {
                AccountManager.getAccount(channel.getPlayer(this.readUD())._playerId, true)?.SendCompletePacket(completeBytes);
              }
              catch
              {
              }
            }
          }
        }
      }
      else
        this.erro = 2147483648U;
      this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_INVITE_LOBBY_USER_LIST_ACK(this.erro));
    }
  }
}
