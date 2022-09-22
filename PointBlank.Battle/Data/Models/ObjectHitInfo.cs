using PointBlank.Battle.Data.Enums;
using SharpDX;

namespace PointBlank.Battle.Data.Models
{
  public class ObjectHitInfo
  {
    public CHARA_DEATH DeathType = CHARA_DEATH.DEFAULT;
    public int Type;
    public int ObjSyncId;
    public int ObjId;
    public int ObjLife;
    public int WeaponId;
    public int KillerId;
    public int AnimId1;
    public int AnimId2;
    public int DestroyState;
    public int HitPart;
    public byte Extensions;
    public Half3 Position;
    public float SpecialUse;

    public ObjectHitInfo(int Type)
    {
      this.Type = Type;
    }
  }
}
