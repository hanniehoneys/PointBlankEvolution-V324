using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core.Models.Map;
using PointBlank.Core.Network;
using System.Collections.Generic;
using System.Linq;

namespace PointBlank.Auth.Network.ClientPacket
{
  public class PROTOCOL_BASE_GET_MAP_INFO_REQ : ReceivePacket
  {
    public PROTOCOL_BASE_GET_MAP_INFO_REQ(AuthClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
    }

    public override void run()
    {
      this._client.SendPacket((SendPacket) new PROTOCOL_BASE_MAP_RULELIST_ACK());
      IEnumerable<IEnumerable<MapMatch>> mapMatches = MapModel.Matchs.Split<MapMatch>(100);
      int Total = 0;
      foreach (IEnumerable<MapMatch> source in mapMatches)
      {
        Total += 100;
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_MAP_MATCHINGLIST_ACK(source.ToList<MapMatch>(), Total));
      }
    }
  }
}
