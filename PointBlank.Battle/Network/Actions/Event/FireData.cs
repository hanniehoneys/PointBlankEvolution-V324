using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Data.Models.Event;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class FireData
  {
    public static FireDataInfo ReadInfo(
      ActionModel Action,
      ReceivePacket Packet,
      bool Log)
    {
      FireDataInfo fireDataInfo = new FireDataInfo() { Effect = Packet.readC(), Part = Packet.readC(), Index = Packet.readH(), X = Packet.readUH(), Y = Packet.readUH(), Z = Packet.readUH(), Extensions = Packet.readC(), WeaponId = Packet.readD() };
      if (Log)
      {
        Logger.warning("[1] Effect: " + (object) ((int) fireDataInfo.Effect >> 4) + "; Slot: " + (object) ((int) fireDataInfo.Effect & 15));
        Logger.warning("[2] Slot: " + (object) Action.Slot + " FireData: " + (object) fireDataInfo.Effect + ";" + (object) fireDataInfo.Part);
      }
      return fireDataInfo;
    }

    public static void ReadInfo(ReceivePacket Packet)
    {
      Packet.Advance(15);
    }

    public static void WriteInfo(
      SendPacket Send,
      ActionModel Action,
      ReceivePacket Packet,
      bool Log)
    {
      FireDataInfo Info = FireData.ReadInfo(Action, Packet, Log);
      FireData.WriteInfo(Send, Info);
    }

    public static void WriteInfo(SendPacket Send, FireDataInfo Info)
    {
      Send.writeC(Info.Effect);
      Send.writeC(Info.Part);
      Send.writeH(Info.Index);
      Send.writeH(Info.X);
      Send.writeH(Info.Y);
      Send.writeH(Info.Z);
      Send.writeC(Info.Extensions);
      Send.writeD(Info.WeaponId);
    }
  }
}
