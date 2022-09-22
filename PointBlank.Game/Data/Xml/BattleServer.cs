using System.Net;

namespace PointBlank.Game.Data.Xml
{
  public class BattleServer
  {
    public string IP;
    public int Port;
    public int SyncPort;
    public IPEndPoint Connection;

    public BattleServer(string ip, int syncPort)
    {
      this.IP = ip;
      this.SyncPort = syncPort;
      this.Connection = new IPEndPoint(IPAddress.Parse(ip), syncPort);
    }
  }
}
