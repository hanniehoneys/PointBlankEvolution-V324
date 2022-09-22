using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_SELECT_AGE_REQ : ReceivePacket
  {
    private int Year;

    public PROTOCOL_BASE_SELECT_AGE_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.Year = this.readD();
    }

    public override void run()
    {
      Account player = this._client._player;
      int Age = int.Parse(DateTime.Now.ToString("yyyy")) - this.Year / 10000;
      player.age = Age;
      ComDiv.updateDB("accounts", "age", (object) Age, "player_id", (object) player.player_id);
      this._client.SendPacket((SendPacket) new PROTOCOL_BASE_SELECT_AGE_ACK(0, Age));
    }
  }
}
