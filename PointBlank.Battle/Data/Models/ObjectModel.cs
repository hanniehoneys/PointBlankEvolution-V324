using System;
using System.Collections.Generic;

namespace PointBlank.Battle.Data.Models
{
  public class ObjectModel
  {
    public int UpdateId = 1;
    public int Id;
    public int Life;
    public int Animation;
    public int UltraSync;
    public bool NeedSync;
    public bool Destroyable;
    public bool NoInstaSync;
    public List<AnimModel> Animations;
    public List<DeffectModel> Effects;

    public ObjectModel(bool NeedSync)
    {
      this.NeedSync = NeedSync;
      if (NeedSync)
        this.Animations = new List<AnimModel>();
      this.Effects = new List<DeffectModel>();
    }

    public int CheckDestroyState(int life)
    {
      for (int index = this.Effects.Count - 1; index > -1; --index)
      {
        DeffectModel effect = this.Effects[index];
        if (effect.Life >= life)
          return effect.Id;
      }
      return 0;
    }

    public int GetRandomAnimation(Room Room, ObjectInfo Obj)
    {
      if (this.Animations != null && this.Animations.Count > 0)
      {
        AnimModel animation = this.Animations[new Random().Next(this.Animations.Count)];
        Obj.Animation = animation;
        Obj.UseDate = DateTime.Now;
        if (animation.OtherObj > 0)
        {
          ObjectInfo objectInfo = Room.Objects[animation.OtherObj];
          this.GetAnim(animation.OtherAnim, 0.0f, 0.0f, objectInfo);
        }
        return animation.Id;
      }
      Obj.Animation = (AnimModel) null;
      return (int) byte.MaxValue;
    }

    public void GetAnim(int animId, float time, float duration, ObjectInfo obj)
    {
      if (animId == (int) byte.MaxValue || obj == null || (obj.Model == null || obj.Model.Animations == null) || obj.Model.Animations.Count == 0)
        return;
      ObjectModel model = obj.Model;
      for (int index = 0; index < model.Animations.Count; ++index)
      {
        AnimModel animation = model.Animations[index];
        if (animation.Id == animId)
        {
          obj.Animation = animation;
          time -= duration;
          obj.UseDate = DateTime.Now.AddSeconds((double) time * -1.0);
          break;
        }
      }
    }
  }
}
