using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using System;
using System.Threading.Tasks;

namespace PointBlank.Auth
{
  public class Auth
  {
    public static async void Update()
    {
      while (true)
      {
        Console.Title = "Point Blank - Auth [Users: " + (object) AuthManager._socketList.Count + " Online: " + (object) ServersXml.getServer(0)._LastCount + " Used RAM: " + (object) (GC.GetTotalMemory(true) / 1024L) + " KB]";
        ComDiv.updateDB("onlines", "auth", (object) ServersXml.getServer(0)._LastCount);
        await Task.Delay(1000);
      }
    }
  }
}
