using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Data.Models.Event;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class ActionState
  {
    public static ActionStateInfo ReadInfo(
      ReceivePacket p,
      ActionModel ac,
      bool genLog)
    {
      ActionStateInfo actionStateInfo = new ActionStateInfo() { Action = p.readUD() };
      if (!genLog);
      return actionStateInfo;
    }

    public static void WriteInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
    {
      ActionStateInfo info = ActionState.ReadInfo(p, ac, genLog);
      ActionState.WriteInfo(s, info);
    }

    public static void WriteInfo(SendPacket s, ActionStateInfo info)
    {
      s.writeD(info.Action);
    }
  }
}
