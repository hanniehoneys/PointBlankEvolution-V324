using PointBlank.Battle.Data;
using PointBlank.Battle.Data.Enums;
using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Network.Actions.SubHead;
using System;
using System.IO;

namespace PointBlank.Battle.Network.Packets
{
  public class PROTOCOL_BOTS_ACTION
  {
    public static byte[] getBaseData(byte[] data)
    {
      ReceivePacket p = new ReceivePacket(data);
      using (SendPacket s = new SendPacket())
      {
        s.writeT(p.readT());
        for (int index = 0; index < 16; ++index)
        {
          ActionModel actionModel = new ActionModel();
          try
          {
            bool exception;
            actionModel.SubHead = (UDP_SUB_HEAD) p.readC(out exception);
            if (!exception)
            {
              actionModel.Slot = p.readUH();
              actionModel.Length = p.readUH();
              if (actionModel.Length != ushort.MaxValue)
              {
                s.writeC((byte) actionModel.SubHead);
                s.writeH(actionModel.Slot);
                s.writeH(actionModel.Length);
                if (actionModel.SubHead == UDP_SUB_HEAD.GRENADE)
                  GrenadeSync.WriteInfo(s, p);
                else if (actionModel.SubHead == UDP_SUB_HEAD.DROPEDWEAPON)
                  DropedWeapon.WriteInfo(s, p);
                else if (actionModel.SubHead == UDP_SUB_HEAD.OBJECT_STATIC)
                  ObjectStatic.WriteInfo(s, p);
                else if (actionModel.SubHead == UDP_SUB_HEAD.OBJECT_ANIM)
                  ObjectAnim.WriteInfo(s, p);
                else if (actionModel.SubHead == UDP_SUB_HEAD.STAGEINFO_OBJ_STATIC)
                  StageInfoObjStatic.WriteInfo(s, p, false);
                else if (actionModel.SubHead == UDP_SUB_HEAD.STAGEINFO_OBJ_ANIM)
                  StageObjAnim.WriteInfo(s, p);
                else if (actionModel.SubHead == UDP_SUB_HEAD.CONTROLED_OBJECT)
                  ControledObj.WriteInfo(s, p, false);
                else if (actionModel.SubHead == UDP_SUB_HEAD.USER || actionModel.SubHead == UDP_SUB_HEAD.STAGEINFO_CHARA)
                {
                  actionModel.Flag = (UDP_GAME_EVENTS) p.readUD();
                  actionModel.Data = p.readB((int) actionModel.Length - 9);
                  s.writeD((uint) actionModel.Flag);
                  s.writeB(actionModel.Data);
                  if (actionModel.Data.Length == 0 && actionModel.Flag > (UDP_GAME_EVENTS) 0)
                    break;
                }
                else
                {
                  Logger.warning("[New User Packet Type: '" + (object) actionModel.SubHead + "' or '" + (object) (int) actionModel.SubHead + "']: " + BitConverter.ToString(data));
                  throw new Exception("Unknown Action Type[2]");
                }
              }
              else
                break;
            }
            else
              break;
          }
          catch (Exception ex)
          {
            Logger.warning("Buffer: " + BitConverter.ToString(data));
            Logger.warning(ex.ToString());
            s.mstream = new MemoryStream();
            break;
          }
        }
        return s.mstream.ToArray();
      }
    }

    public static byte[] getCode(byte[] data, DateTime time, int round, int slot)
    {
      using (SendPacket sendPacket = new SendPacket())
      {
        byte[] baseData = PROTOCOL_BOTS_ACTION.getBaseData(data);
        sendPacket.writeC((byte) 132);
        sendPacket.writeC((byte) slot);
        sendPacket.writeT(AllUtils.GetDuration(time));
        sendPacket.writeC((byte) round);
        sendPacket.writeH((ushort) (13 + baseData.Length));
        sendPacket.writeD(0);
        sendPacket.writeB(AllUtils.Encrypt(baseData, (13 + baseData.Length) % 6 + 1));
        return sendPacket.mstream.ToArray();
      }
    }
  }
}
