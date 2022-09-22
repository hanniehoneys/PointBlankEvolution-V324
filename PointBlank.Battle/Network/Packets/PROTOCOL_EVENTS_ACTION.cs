using PointBlank.Battle.Data;
using PointBlank.Battle.Data.Enums;
using PointBlank.Battle.Data.Models;
using System;
using System.Collections.Generic;

namespace PointBlank.Battle.Network.Packets
{
  public class PROTOCOL_EVENTS_ACTION
  {
    public static byte[] getCode(byte[] actions, DateTime date, int round, int slot)
    {
      return PROTOCOL_EVENTS_ACTION.BaseGetCode(AllUtils.Encrypt(actions, (13 + actions.Length) % 6 + 1), date, round, slot);
    }

    private static byte[] BaseGetCode(byte[] actionsBuffer, DateTime date, int round, int slot)
    {
      using (SendPacket sendPacket = new SendPacket())
      {
        sendPacket.writeC((byte) 4);
        sendPacket.writeC((byte) slot);
        sendPacket.writeT(AllUtils.GetDuration(date));
        sendPacket.writeC((byte) round);
        sendPacket.writeH((ushort) (13 + actionsBuffer.Length));
        sendPacket.writeD(0);
        sendPacket.writeB(actionsBuffer);
        return sendPacket.mstream.ToArray();
      }
    }

    public static byte[] getCodeSyncData(List<ObjectHitInfo> objs)
    {
      using (SendPacket sendPacket = new SendPacket())
      {
        for (int index = 0; index < objs.Count; ++index)
        {
          ObjectHitInfo objectHitInfo = objs[index];
          if (objectHitInfo.Type == 1)
          {
            if (objectHitInfo.ObjSyncId == 0)
            {
              sendPacket.writeC((byte) 3);
              sendPacket.writeH((ushort) objectHitInfo.ObjId);
              sendPacket.writeH((short) 10);
              sendPacket.writeH((ushort) 6);
              sendPacket.writeH((ushort) objectHitInfo.ObjLife);
              sendPacket.writeC((byte) objectHitInfo.KillerId);
            }
            else
            {
              sendPacket.writeC((byte) 3);
              sendPacket.writeH((ushort) objectHitInfo.ObjId);
              sendPacket.writeH((short) 13);
              sendPacket.writeH((ushort) objectHitInfo.ObjLife);
              sendPacket.writeC((byte) objectHitInfo.AnimId1);
              sendPacket.writeC((byte) objectHitInfo.AnimId2);
              sendPacket.writeT(objectHitInfo.SpecialUse);
            }
          }
          else if (objectHitInfo.Type == 2)
          {
            UDP_GAME_EVENTS udpGameEvents = UDP_GAME_EVENTS.HpSync;
            int num = 11;
            if (objectHitInfo.ObjLife == 0)
            {
              udpGameEvents |= UDP_GAME_EVENTS.GetWeaponForHost;
              num += 12;
            }
            sendPacket.writeC((byte) 0);
            sendPacket.writeH((ushort) objectHitInfo.ObjId);
            sendPacket.writeH((ushort) num);
            sendPacket.writeD((uint) udpGameEvents);
            sendPacket.writeH((ushort) objectHitInfo.ObjLife);
            if (udpGameEvents.HasFlag((Enum) UDP_GAME_EVENTS.GetWeaponForHost))
            {
              sendPacket.writeC((byte) objectHitInfo.DeathType);
              sendPacket.writeC((byte) objectHitInfo.HitPart);
              sendPacket.writeH(objectHitInfo.Position.X.RawValue);
              sendPacket.writeH(objectHitInfo.Position.Y.RawValue);
              sendPacket.writeH(objectHitInfo.Position.Z.RawValue);
              sendPacket.writeD(objectHitInfo.WeaponId);
            }
          }
          else if (objectHitInfo.Type == 3)
          {
            if (objectHitInfo.ObjSyncId == 0)
            {
              sendPacket.writeC((byte) 3);
              sendPacket.writeH((ushort) objectHitInfo.ObjId);
              sendPacket.writeH((short) 8);
              sendPacket.writeH((short) 1);
              sendPacket.writeC(objectHitInfo.ObjLife == 0);
            }
            else
            {
              sendPacket.writeC((byte) 3);
              sendPacket.writeH((ushort) objectHitInfo.ObjId);
              sendPacket.writeH((short) 14);
              sendPacket.writeC((byte) objectHitInfo.DestroyState);
              sendPacket.writeH((ushort) objectHitInfo.ObjLife);
              sendPacket.writeT(objectHitInfo.SpecialUse);
              sendPacket.writeC((byte) objectHitInfo.AnimId1);
              sendPacket.writeC((byte) objectHitInfo.AnimId2);
            }
          }
          else if (objectHitInfo.Type == 4)
          {
            sendPacket.writeC((byte) 8);
            sendPacket.writeH((ushort) objectHitInfo.ObjId);
            sendPacket.writeH((short) 11);
            sendPacket.writeD(256);
            sendPacket.writeH((ushort) objectHitInfo.ObjLife);
          }
        }
        return sendPacket.mstream.ToArray();
      }
    }
  }
}
