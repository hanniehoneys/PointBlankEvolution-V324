using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Data.Models.Event;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class Animation
  {
    public static AnimationInfo ReadInfo(ActionModel ac, ReceivePacket p, bool genLog)
    {
      AnimationInfo animationInfo = new AnimationInfo() { Animation = p.readUH() };
      if (genLog)
        Logger.warning("Slot: " + (object) ac.Slot + " is moving the crosshair position: posV (" + (object) animationInfo.Animation + ")");
      return animationInfo;
    }

    public static void WriteInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
    {
      AnimationInfo animationInfo = Animation.ReadInfo(ac, p, genLog);
      s.writeH(animationInfo.Animation);
    }
  }
}
