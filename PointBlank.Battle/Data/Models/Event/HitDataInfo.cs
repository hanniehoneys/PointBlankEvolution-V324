using PointBlank.Battle.Data.Enums;
using SharpDX;
using System.Collections.Generic;

namespace PointBlank.Battle.Data.Models.Event
{
  public class HitDataInfo
  {
    public byte Extensions;
    public ushort BoomInfo;
    public uint HitIndex;
    public int WeaponId;
    public Half3 StartBullet;
    public Half3 EndBullet;
    public List<int> BoomPlayers;
    public HIT_TYPE HitEnum;
    public CLASS_TYPE WeaponClass;
  }
}
