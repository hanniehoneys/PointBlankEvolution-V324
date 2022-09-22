using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using System.Collections.Generic;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_AUTH_FRIEND_INFO_ACK : SendPacket
  {
    private List<Friend> friends;

    public PROTOCOL_AUTH_FRIEND_INFO_ACK(List<Friend> friends)
    {
      this.friends = friends;
    }

    public override void write()
    {
      this.writeH((short) 786);
      this.writeC((byte) this.friends.Count);
      for (int index = 0; index < this.friends.Count; ++index)
      {
        Friend friend = this.friends[index];
        PlayerInfo player = friend.player;
        if (player == null)
        {
          this.writeB(new byte[15]);
        }
        else
        {
          this.writeC((byte) (player.player_name.Length + 1));
          this.writeUnicode(player.player_name, true);
          this.writeQ(player.player_id);
          this.writeD(ComDiv.GetFriendStatus(friend));
          this.writeC((byte) player._rank);
          this.writeC((byte) 0);
        }
      }
    }
  }
}
