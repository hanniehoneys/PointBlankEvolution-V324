using System;

namespace PointBlank.Battle.Data.Models
{
  public class ObjectInfo
  {
    public int Life = 100;
    public int Id;
    public int DestroyState;
    public AnimModel Animation;
    public DateTime UseDate;
    public ObjectModel Model;

    public ObjectInfo()
    {
    }

    public ObjectInfo(int Id)
    {
      this.Id = Id;
    }
  }
}
