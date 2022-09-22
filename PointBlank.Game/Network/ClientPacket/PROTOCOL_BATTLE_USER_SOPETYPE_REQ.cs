using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_USER_SOPETYPE_REQ : ReceivePacket
  {
    private int Sight;

    public PROTOCOL_BATTLE_USER_SOPETYPE_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.Sight = (int) this.readC();
    }

    public override void run()
    {
      Account player = this._client._player;
      Room room = player._room;
      if (player == null)
        return;
      player.Sight = this.Sight;
    }
  }
}
