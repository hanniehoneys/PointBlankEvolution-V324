namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CLAN_WAR_RESULT_REQ : ReceivePacket
  {
    private int ClanId;

    public PROTOCOL_CLAN_WAR_RESULT_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.ClanId = this.readD();
    }

    public override void run()
    {
    }
  }
}
