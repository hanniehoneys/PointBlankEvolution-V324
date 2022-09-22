namespace PointBlank.Battle.Network.Packets
{
  public class PROTOCOL_CONNECT
  {
    public static byte[] getCode()
    {
      using (SendPacket sendPacket = new SendPacket())
      {
        sendPacket.writeC((byte) 66);
        sendPacket.writeC((byte) 0);
        sendPacket.writeT(0.0f);
        sendPacket.writeC((byte) 0);
        sendPacket.writeH((short) 14);
        sendPacket.writeD(0);
        sendPacket.writeC((byte) 8);
        return sendPacket.mstream.ToArray();
      }
    }
  }
}
