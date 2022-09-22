using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_HOLE_CHECK_REQ : ReceivePacket
  {
    public PROTOCOL_BATTLE_HOLE_CHECK_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
    }

    public override void run()
    {
      try
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_HOLE_CHECK_ACK());
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }
  }
}
