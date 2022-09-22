using PointBlank.Battle.Data.Enums;
using SharpDX;
using System.Collections.Generic;

namespace PointBlank.Battle.Data.Models.Event
{
  public class GrenadeHitInfo
  {
    public byte DeathType;
    public byte Extensions;
    public ushort BoomInfo;
    public ushort GrenadesCount;
    public uint HitInfo;
    public int WeaponId;
    public List<int> BoomPlayers;
    public Half3 FirePos;
    public Half3 HitPos;
    public Half3 PlayerPos;
    public HIT_TYPE HitEnum;
    public CLASS_TYPE WeaponClass;
  }
}
