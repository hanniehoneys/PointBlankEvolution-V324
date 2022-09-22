using PointBlank.Core.Managers.Server;
using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_NOTICE_ACK : SendPacket
  {
    public override void write()
    {
      ServerConfig config = AuthManager.Config;
      this.writeH((short) 662);
      this.writeH((short) 0);
      this.writeD(config.ChatColor);
      this.writeD(config.AnnouceColor);
      this.writeH((ushort) config.Chat.Length);
      this.writeText(config.Chat, config.Chat.Length);
      this.writeH((ushort) config.Annouce.Length);
      this.writeText(config.Annouce, config.Annouce.Length);
    }
  }
}
